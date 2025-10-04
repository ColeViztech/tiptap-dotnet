using System;
using System.Linq;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using NodeBase = Tiptap.Core.Core.Node;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class CodeBlock : NodeBase
{
    public CodeBlock(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "codeBlock";

    public override string Marks => string.Empty;

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["languageClassPrefix"] = "language-",
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "pre",
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        return new Dictionary<string, AttributeConfiguration>
        {
            ["language"] = new AttributeConfiguration
            {
                ParseHTML = domNode =>
                {
                    if (domNode is not IElement element)
                    {
                        return null;
                    }

                    var firstChild = element.Children.FirstOrDefault();
                    if (firstChild == null)
                    {
                        return null;
                    }

                    var className = firstChild.GetAttribute("class");
                    if (string.IsNullOrWhiteSpace(className))
                    {
                        return null;
                    }

                    var prefix = this.GetStringOption("languageClassPrefix", "language-");
                    return className.StartsWith(prefix, StringComparison.Ordinal)
                        ? className[prefix.Length..]
                        : null;
                },
                Rendered = false,
            },
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        var language = node.Attrs != null && node.Attrs.TryGetValue("language", out var value)
            ? value?.ToString()
            : null;

        var prefix = this.GetStringOption("languageClassPrefix", "language-");
        var codeAttributes = new Dictionary<string, object?>
        {
            ["class"] = !string.IsNullOrWhiteSpace(language)
                ? prefix + language
                : null,
        };

        return new object?[]
        {
            "pre",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            new object?[]
            {
                "code",
                codeAttributes,
                0,
            },
        };
    }
}
