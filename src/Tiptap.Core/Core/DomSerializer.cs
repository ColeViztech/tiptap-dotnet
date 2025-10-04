using System.Linq;
using System.Net;
using System.Text;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Core;

public class DomSerializer
{
    private readonly Schema _schema;
    private static readonly HashSet<string> SelfClosingTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr",
    };

    public DomSerializer(Schema schema)
    {
        _schema = schema;
    }

    public string Process(object value)
    {
        var document = Serialization.ToDocument(value);
        var html = new List<string>();
        var content = document.Content ?? new List<ProseMirrorNode>();
        var markStack = new List<(Mark Extension, ProseMirrorMark Mark)>();

        for (var index = 0; index < content.Count; index++)
        {
            var node = content[index];
            var previousNode = index > 0 ? content[index - 1] : null;
            var nextNode = index + 1 < content.Count ? content[index + 1] : null;

            html.Add(RenderNode(node, previousNode, nextNode, markStack));
        }

        return string.Join(string.Empty, html);
    }

    private string RenderNode(ProseMirrorNode node, ProseMirrorNode? previousNode, ProseMirrorNode? nextNode, List<(Mark Extension, ProseMirrorMark Mark)> markStack)
    {
        var html = new List<string>();
        var markTagsToClose = new List<(Mark Extension, ProseMirrorMark Mark)>();

        if (node.Marks != null)
        {
            foreach (var mark in node.Marks)
            {
                foreach (var markExtension in _schema.Marks)
                {
                    if (!IsMarkOrNode(mark, markExtension))
                    {
                        continue;
                    }

                    if (!MarkShouldOpen(mark, previousNode))
                    {
                        continue;
                    }

                    html.Add(RenderOpeningTag(markExtension, mark));
                    markStack.Add((markExtension, mark));
                }
            }
        }

        Node? matchedNodeExtension = null;
        foreach (var nodeExtension in _schema.Nodes)
        {
            if (!IsMarkOrNode(node, nodeExtension))
            {
                continue;
            }

            html.Add(RenderOpeningTag(nodeExtension, node));
            matchedNodeExtension = nodeExtension;
            break;
        }

        if (node.Content != null && node.Content.Count > 0)
        {
            var nestedMarkStack = new List<(Mark Extension, ProseMirrorMark Mark)>();
            for (var index = 0; index < node.Content.Count; index++)
            {
                var nestedNode = node.Content[index];
                var previousNestedNode = index > 0 ? node.Content[index - 1] : null;
                var nextNestedNode = index + 1 < node.Content.Count ? node.Content[index + 1] : null;

                html.Add(RenderNode(nestedNode, previousNestedNode, nextNestedNode, nestedMarkStack));
            }
        }
        else if (matchedNodeExtension != null && matchedNodeExtension.RenderText(node) is { } renderedText && !string.IsNullOrEmpty(renderedText))
        {
            html.Add(renderedText);
        }
        else if (!string.IsNullOrEmpty(node.Text))
        {
            html.Add(WebUtility.HtmlEncode(node.Text));
        }

        if (matchedNodeExtension != null)
        {
            var closing = RenderClosingTag(matchedNodeExtension.RenderHTML(node));
            if (!string.IsNullOrEmpty(closing))
            {
                html.Add(closing);
            }
        }

        if (node.Marks != null)
        {
            foreach (var mark in node.Marks.AsEnumerable().Reverse())
            {
                foreach (var markExtension in _schema.Marks)
                {
                    if (!IsMarkOrNode(mark, markExtension))
                    {
                        continue;
                    }

                    if (!MarkShouldClose(mark, nextNode))
                    {
                        continue;
                    }

                    markTagsToClose.Add((markExtension, mark));
                }
            }

            if (markTagsToClose.Count > 0)
            {
                html.AddRange(CloseAndReopenTags(markTagsToClose, markStack));
            }
        }

        return string.Join(string.Empty, html);
    }

    private IEnumerable<string> CloseAndReopenTags(List<(Mark Extension, ProseMirrorMark Mark)> markTagsToClose, List<(Mark Extension, ProseMirrorMark Mark)> markStack)
    {
        var markTagsToReopen = new List<(Mark Extension, ProseMirrorMark Mark)>();
        var closingTags = CloseMarkTags(markTagsToClose, markStack, markTagsToReopen).ToList();
        var reopeningTags = ReopenMarkTags(markTagsToReopen, markStack).ToList();

        return closingTags.Concat(reopeningTags);
    }

    private IEnumerable<string> CloseMarkTags(List<(Mark Extension, ProseMirrorMark Mark)> markTagsToClose, List<(Mark Extension, ProseMirrorMark Mark)> markStack, List<(Mark Extension, ProseMirrorMark Mark)> markTagsToReopen)
    {
        var html = new List<string>();

        while (markTagsToClose.Count > 0 && markStack.Count > 0)
        {
            var markTag = markStack[^1];
            markStack.RemoveAt(markStack.Count - 1);

            var markExtension = markTag.Extension;
            var mark = markTag.Mark;

            var closing = RenderClosingTag(markExtension.RenderHTML(mark));
            if (!string.IsNullOrEmpty(closing))
            {
                html.Add(closing);
            }

            var index = markTagsToClose.FindIndex(item => item.Extension == markExtension && MarksEqual(item.Mark, mark));
            if (index >= 0)
            {
                markTagsToClose.RemoveAt(index);
            }
            else
            {
                markTagsToReopen.Add(markTag);
            }
        }

        markTagsToClose.Clear();
        return html;
    }

    private IEnumerable<string> ReopenMarkTags(IEnumerable<(Mark Extension, ProseMirrorMark Mark)> markTagsToReopen, List<(Mark Extension, ProseMirrorMark Mark)> markStack)
    {
        var html = new List<string>();

        foreach (var markTag in markTagsToReopen.Reverse())
        {
            html.Add(RenderOpeningTag(markTag.Extension, markTag.Mark));
            markStack.Add(markTag);
        }

        return html;
    }

    private bool IsMarkOrNode(ProseMirrorMark mark, Mark renderClass)
    {
        return mark.Type != null && mark.Type.Equals(renderClass.Name, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsMarkOrNode(ProseMirrorNode node, Node renderClass)
    {
        return node.Type != null && node.Type.Equals(renderClass.Name, StringComparison.OrdinalIgnoreCase);
    }

    private bool MarkShouldOpen(ProseMirrorMark mark, ProseMirrorNode? previousNode)
    {
        return NodeHasMark(previousNode, mark);
    }

    private bool MarkShouldClose(ProseMirrorMark mark, ProseMirrorNode? nextNode)
    {
        return NodeHasMark(nextNode, mark);
    }

    private bool NodeHasMark(ProseMirrorNode? node, ProseMirrorMark mark)
    {
        if (node == null || node.Marks == null)
        {
            return true;
        }

        return node.Marks.All(other => !MarksEqual(other, mark));
    }

    private bool MarksEqual(ProseMirrorMark a, ProseMirrorMark b)
    {
        if (!string.Equals(a.Type, b.Type, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (a.Attrs == null && b.Attrs == null)
        {
            return true;
        }

        if (a.Attrs == null || b.Attrs == null)
        {
            return false;
        }

        if (a.Attrs.Count != b.Attrs.Count)
        {
            return false;
        }

        foreach (var (key, value) in a.Attrs)
        {
            if (!b.Attrs.TryGetValue(key, out var otherValue))
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

    private string RenderOpeningTag(Extension extension, object nodeOrMark, object? renderHtml = null)
    {
        IDictionary<string, object?> htmlAttributes = new Dictionary<string, object?>();
        foreach (var (attribute, configuration) in _schema.GetAttributeConfigurations(extension))
        {
            if (configuration.Rendered == false)
            {
                continue;
            }

            var attrs = EnsureAttributes(nodeOrMark);

            if (!attrs.ContainsKey(attribute) && configuration.Default is not null)
            {
                attrs[attribute] = configuration.Default;
            }

            if (configuration.RenderHTML != null)
            {
                var value = configuration.RenderHTML(attrs);
                if (value != null)
                {
                    htmlAttributes = HtmlUtilities.MergeAttributes(htmlAttributes, value);
                }
            }
            else
            {
                attrs.TryGetValue(attribute, out var attributeValue);
                var value = new Dictionary<string, object?>
                {
                    [attribute] = attributeValue,
                };
                htmlAttributes = HtmlUtilities.MergeAttributes(htmlAttributes, value);
            }
        }

        htmlAttributes = htmlAttributes
            .Where(pair => pair.Value != null)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        renderHtml ??= extension switch
        {
            Node node when nodeOrMark is ProseMirrorNode proseMirrorNode => node.RenderHTML(proseMirrorNode),
            Mark mark when nodeOrMark is ProseMirrorMark proseMirrorMark => mark.RenderHTML(proseMirrorMark),
            _ => null,
        };

        if (renderHtml is IDictionary<string, object?> dict && dict.TryGetValue("content", out var content) && content is string contentString)
        {
            return contentString;
        }

        if (renderHtml is null)
        {
            return string.Empty;
        }

        if (renderHtml is string tagName)
        {
            var attributes = htmlAttributes.Count > 0
                ? HtmlUtilities.RenderAttributes(htmlAttributes)
                : string.Empty;

            return $"<{tagName}{attributes}>";
        }

        if (renderHtml is IEnumerable<object?> enumerable)
        {
            var instructions = enumerable.ToList();
            var builder = new StringBuilder();

            for (var index = 0; index < instructions.Count; index++)
            {
                var instruction = instructions[index];

                switch (instruction)
                {
                    case string tag:
                        IDictionary<string, object?>? attributes = null;
                        if (index + 1 < instructions.Count && instructions[index + 1] is IDictionary<string, object?> nextAttributes && !ContainsZero(nextAttributes.Values.Cast<object?>()))
                        {
                            attributes = nextAttributes;
                            index++;
                        }

                        var mergedAttributes = attributes != null
                            ? HtmlUtilities.MergeAttributes(htmlAttributes, attributes)
                            : htmlAttributes;

                        var attributeString = mergedAttributes.Count > 0
                            ? HtmlUtilities.RenderAttributes(mergedAttributes)
                            : string.Empty;

                        builder.Append($"<{tag}{attributeString}>");
                        break;
                    case IEnumerable<object?> nested when ContainsZero(nested):
                        builder.Append(RenderOpeningTag(extension, nodeOrMark, nested));
                        break;
                }
            }

            return builder.ToString();
        }

        throw new InvalidOperationException($"[renderOpeningTag] Failed to use renderHTML: {System.Text.Json.JsonSerializer.Serialize(renderHtml)}");
    }

    private string RenderClosingTag(object? renderHtml)
    {
        if (renderHtml is null)
        {
            return string.Empty;
        }

        if (renderHtml is IDictionary<string, object?> dict && dict.ContainsKey("content"))
        {
            return string.Empty;
        }

        if (renderHtml is string tag)
        {
            return IsSelfClosing(tag) ? string.Empty : $"</{tag}>";
        }

        if (renderHtml is IEnumerable<object?> enumerable)
        {
            var instructions = enumerable.ToList();
            var builder = new StringBuilder();

            for (var index = instructions.Count - 1; index >= 0; index--)
            {
                var instruction = instructions[index];
                switch (instruction)
                {
                    case string tagName:
                        if (IsSelfClosing(tagName))
                        {
                            continue;
                        }

                        builder.Append($"</{tagName}>");
                        break;
                    case IEnumerable<object?> nested when ContainsZero(nested):
                        builder.Append(RenderClosingTag(nested));
                        break;
                }
            }

            return builder.ToString();
        }

        throw new InvalidOperationException($"[renderClosingTag] Failed to use renderHTML: {System.Text.Json.JsonSerializer.Serialize(renderHtml)}");
    }

    private static bool ContainsZero(IEnumerable<object?> values)
    {
        return values.Any(value => value is int intValue && intValue == 0);
    }

    private static bool IsSelfClosing(string tag)
    {
        return SelfClosingTags.Contains(tag);
    }

    private static Dictionary<string, object?> EnsureAttributes(object nodeOrMark)
    {
        switch (nodeOrMark)
        {
            case ProseMirrorNode node:
                node.Attrs ??= new Dictionary<string, object?>();
                return node.Attrs;
            case ProseMirrorMark mark:
                mark.Attrs ??= new Dictionary<string, object?>();
                return mark.Attrs;
            default:
                return new Dictionary<string, object?>();
        }
    }
}
