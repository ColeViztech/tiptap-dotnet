using System;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Core;

public class DomParser
{
    private readonly Schema _schema;
    private readonly Minify _minify = new();
    private IDocument? _document;
    private readonly List<ProseMirrorMark> _storedMarks = new();
    private static readonly Regex TagPattern = new(@"([a-zA-Z-]*)\[([a-z-]+)(=""?([a-zA-Z]*)""?)?\]$", RegexOptions.Compiled);
    private static readonly Regex StylePattern = new(@"([a-zA-Z-]*)(=""?([a-zA-Z-]*)""?)?$", RegexOptions.Compiled);

    public DomParser(Schema schema)
    {
        _schema = schema;
    }

    public ProseMirrorDocument Process(string value)
    {
        SetDocument(value);
        var content = ProcessChildren(GetDocumentBody());

        return new ProseMirrorDocument
        {
            Type = _schema.TopNode?.Name,
            Content = content,
        };
    }

    private void SetDocument(string value)
    {
        var parser = new HtmlParser();
        var minified = _minify.Process(value);
        _document = parser.ParseDocument($"<?xml encoding=\"utf-8\" ?>{minified}");
    }

    private IElement GetDocumentBody()
    {
        if (_document?.Body == null)
        {
            throw new InvalidOperationException("The HTML document does not contain a body element.");
        }

        return _document.Body;
    }

    private List<ProseMirrorNode> ProcessChildren(INode node)
    {
        var nodes = new List<ProseMirrorNode>();

        foreach (var child in node.ChildNodes)
        {
            var nodeExtension = GetNodeFor(child);
            if (nodeExtension != null)
            {
                var item = ParseNodeAttributes(nodeExtension, child);
                if (item == null)
                {
                    if (child.HasChildNodes && !ShouldSkipNode(child))
                    {
                        nodes.AddRange(ProcessChildren(child));
                    }

                    continue;
                }

                if (child.HasChildNodes)
                {
                    item.Content = ProcessChildren(child);
                }

                if (_storedMarks.Count > 0)
                {
                    item.Marks = _storedMarks.Select(mark => mark.Clone()).ToList();
                }

                nodes.Add(item);
                continue;
            }

            var markExtension = GetMarkFor(child);
            if (markExtension != null)
            {
                var mark = ParseMarkAttributes(markExtension, child);
                if (mark != null)
                {
                    _storedMarks.Add(mark);
                }

                if (child.HasChildNodes && !ShouldSkipNode(child))
                {
                    nodes.AddRange(ProcessChildren(child));
                }

                if (mark != null)
                {
                    _storedMarks.RemoveAt(_storedMarks.Count - 1);
                }

                continue;
            }

            if (child.HasChildNodes && !ShouldSkipNode(child))
            {
                nodes.AddRange(ProcessChildren(child));
            }
        }

        return MergeSimilarNodes(nodes);
    }

    private static bool ShouldSkipNode(INode node)
    {
        return string.Equals(node.NodeName, "script", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.NodeName, "style", StringComparison.OrdinalIgnoreCase);
    }

    private Node? GetNodeFor(INode item)
    {
        return GetExtensionFor(item, _schema.Nodes);
    }

    private Mark? GetMarkFor(INode item)
    {
        return GetExtensionFor(item, _schema.Marks);
    }

    private TExtension? GetExtensionFor<TExtension>(INode node, IEnumerable<TExtension> classes)
        where TExtension : Extension
    {
        var parseRules = new List<ParseRule>();

        foreach (var extension in classes)
        {
            var classParseRules = GetClassParseRules(extension, node);
            parseRules.AddRange(classParseRules);
        }

        parseRules = parseRules
            .OrderByDescending(rule => rule.Priority)
            .ToList();

        foreach (var parseRule in parseRules)
        {
            if (CheckParseRule(parseRule, node))
            {
                return (TExtension?)parseRule.Class;
            }
        }

        return null;
    }

    private IEnumerable<ParseRule> GetClassParseRules(Extension extension, INode node)
    {
        var parseRules = extension switch
        {
            Node nodeExtension => nodeExtension.ParseHTML(node),
            Mark markExtension => markExtension.ParseHTML(node),
            _ => Enumerable.Empty<ParseRule>(),
        };

        if (parseRules == null)
        {
            return Enumerable.Empty<ParseRule>();
        }

        return parseRules
            .Select(rule =>
            {
                rule.Class = extension;
                return rule;
            })
            .ToList();
    }

    private bool CheckParseRule(ParseRule parseRule, INode domNode)
    {
        if (parseRule.Tag != null)
        {
            var tagMatch = TagPattern.Match(parseRule.Tag);
            string? tagName;
            string? attribute = null;
            string? attributeValue = null;

            if (tagMatch.Success)
            {
                tagName = tagMatch.Groups[1].Value;
                attribute = tagMatch.Groups[2].Value;
                if (tagMatch.Groups.Count >= 5)
                {
                    var value = tagMatch.Groups[4].Value;
                    attributeValue = string.IsNullOrEmpty(value) ? null : value;
                }
            }
            else
            {
                tagName = parseRule.Tag;
            }

            if (domNode is IElement element)
            {
                if (!string.Equals(tagName, element.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (attribute != null && !element.HasAttribute(attribute))
                {
                    return false;
                }

                if (attribute != null && attributeValue != null && !string.Equals(element.GetAttribute(attribute), attributeValue, StringComparison.Ordinal))
                {
                    return false;
                }
            }
            else
            {
                if (!string.Equals(tagName, domNode.NodeName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (attribute != null)
                {
                    return false;
                }
            }
        }

        if (parseRule.Style != null)
        {
            var (style, value) = ParseStyleExpression(parseRule.Style);

            if (!InlineStyle.HasAttribute(domNode, style))
            {
                return false;
            }

            if (value != null && InlineStyle.GetAttribute(domNode, style) != value)
            {
                return false;
            }
        }

        if (parseRule.GetAttrs != null)
        {
            object? parameter;
            if (parseRule.Style != null)
            {
                var (style, _) = ParseStyleExpression(parseRule.Style);
                parameter = InlineStyle.HasAttribute(domNode, style)
                    ? InlineStyle.GetAttribute(domNode, style)
                    : domNode;
            }
            else
            {
                parameter = domNode;
            }

            if (parseRule.GetAttrs(parameter) is bool boolean && boolean == false)
            {
                return false;
            }
        }

        if (parseRule.Tag == null && parseRule.Style == null && parseRule.GetAttrs == null)
        {
            return false;
        }

        return true;
    }

    private ProseMirrorNode? ParseNodeAttributes(Node extension, INode domNode)
    {
        var item = new ProseMirrorNode
        {
            Type = extension.Name,
        };

        if (string.Equals(extension.Name, "text", StringComparison.OrdinalIgnoreCase))
        {
            var text = domNode.TextContent?.TrimStart('\n');
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            item.Text = text;
        }

        var parseRules = extension.ParseHTML(domNode);
        if (parseRules != null)
        {
            foreach (var parseRule in parseRules)
            {
                if (!CheckParseRule(parseRule, domNode))
                {
                    continue;
                }

                if (parseRule.Attrs != null && parseRule.Attrs.Count > 0)
                {
                    item.Attrs ??= new Dictionary<string, object?>();
                    foreach (var (key, value) in parseRule.Attrs)
                    {
                        item.Attrs[key] = value;
                    }
                }

                if (parseRule.GetAttrs != null)
                {
                    object? parameter;
                    if (parseRule.Style != null)
                    {
                        var (style, _) = ParseStyleExpression(parseRule.Style);
                        parameter = InlineStyle.HasAttribute(domNode, style)
                            ? InlineStyle.GetAttribute(domNode, style)
                            : domNode;
                    }
                    else
                    {
                        parameter = domNode;
                    }

                    if (parseRule.GetAttrs(parameter) is IDictionary<string, object?> attributes)
                    {
                        item.Attrs ??= new Dictionary<string, object?>();
                        foreach (var (key, value) in attributes)
                        {
                            item.Attrs[key] = value;
                        }
                    }
                }
            }
        }

        foreach (var (attribute, configuration) in _schema.GetAttributeConfigurations(extension))
        {
            object? value;
            if (configuration.ParseHTML != null)
            {
                value = configuration.ParseHTML(domNode);
            }
            else if (domNode is IElement element)
            {
                value = element.GetAttribute(attribute);
            }
            else
            {
                value = null;
            }

            if (value != null)
            {
                item.Attrs ??= new Dictionary<string, object?>();
                item.Attrs[attribute] = value;
            }
        }

        return item;
    }

    private ProseMirrorMark? ParseMarkAttributes(Mark extension, INode domNode)
    {
        var item = new ProseMirrorMark
        {
            Type = extension.Name,
        };

        var parseRules = extension.ParseHTML(domNode);
        if (parseRules == null)
        {
            return item;
        }

        foreach (var parseRule in parseRules)
        {
            if (!CheckParseRule(parseRule, domNode))
            {
                continue;
            }

            if (parseRule.Attrs != null && parseRule.Attrs.Count > 0)
            {
                item.Attrs ??= new Dictionary<string, object?>();
                foreach (var (key, value) in parseRule.Attrs)
                {
                    item.Attrs[key] = value;
                }
            }

            if (parseRule.GetAttrs != null)
            {
                object? parameter;
                if (parseRule.Style != null)
                {
                    var (style, _) = ParseStyleExpression(parseRule.Style);
                    parameter = InlineStyle.HasAttribute(domNode, style)
                        ? InlineStyle.GetAttribute(domNode, style)
                        : domNode;
                }
                else
                {
                    parameter = domNode;
                }

                if (parseRule.GetAttrs(parameter) is IDictionary<string, object?> attributes)
                {
                    item.Attrs ??= new Dictionary<string, object?>();
                    foreach (var (key, value) in attributes)
                    {
                        item.Attrs[key] = value;
                    }
                }
            }
        }

        foreach (var (attribute, configuration) in _schema.GetAttributeConfigurations(extension))
        {
            object? value;
            if (configuration.ParseHTML != null)
            {
                value = configuration.ParseHTML(domNode);
            }
            else if (domNode is IElement element)
            {
                value = element.GetAttribute(attribute);
            }
            else
            {
                value = null;
            }

            if (value != null)
            {
                item.Attrs ??= new Dictionary<string, object?>();
                item.Attrs[attribute] = value;
            }
        }

        return item;
    }

    private List<ProseMirrorNode> MergeSimilarNodes(List<ProseMirrorNode> nodes)
    {
        if (nodes.Count == 0)
        {
            return nodes;
        }

        var result = new List<ProseMirrorNode>();

        foreach (var node in nodes)
        {
            if (result.Count == 0)
            {
                result.Add(node);
                continue;
            }

            var previous = result[^1];
            if (CanMerge(previous, node))
            {
                previous.Text += node.Text;
            }
            else
            {
                result.Add(node);
            }
        }

        return result;
    }

    private bool CanMerge(ProseMirrorNode a, ProseMirrorNode b)
    {
        if (a.Content != null || b.Content != null)
        {
            return false;
        }

        if (a.Text == null || b.Text == null)
        {
            return false;
        }

        if (!string.Equals(a.Type, b.Type, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!AttributesEqual(a.Attrs, b.Attrs))
        {
            return false;
        }

        if (!MarksEqual(a.Marks, b.Marks))
        {
            return false;
        }

        return true;
    }

    private bool AttributesEqual(Dictionary<string, object?>? a, Dictionary<string, object?>? b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        if (a.Count != b.Count)
        {
            return false;
        }

        foreach (var (key, value) in a)
        {
            if (!b.TryGetValue(key, out var otherValue))
            {
                return false;
            }

            if (!Equals(value, otherValue))
            {
                return false;
            }
        }

        return true;
    }

    private bool MarksEqual(List<ProseMirrorMark>? a, List<ProseMirrorMark>? b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        if (a.Count != b.Count)
        {
            return false;
        }

        for (var index = 0; index < a.Count; index++)
        {
            if (!MarksEqual(a[index], b[index]))
            {
                return false;
            }
        }

        return true;
    }

    private bool MarksEqual(ProseMirrorMark a, ProseMirrorMark b)
    {
        if (!string.Equals(a.Type, b.Type, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!AttributesEqual(a.Attrs, b.Attrs))
        {
            return false;
        }

        return true;
    }

    private (string Property, string? Value) ParseStyleExpression(string expression)
    {
        var match = StylePattern.Match(expression);
        if (match.Success)
        {
            var property = match.Groups[1].Value;
            var value = match.Groups.Count >= 4 ? match.Groups[3].Value : null;
            return (property, string.IsNullOrEmpty(value) ? null : value);
        }

        return (expression, null);
    }
}
