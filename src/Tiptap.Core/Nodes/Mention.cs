using System;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using NodeBase = Tiptap.Core.Core.Node;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class Mention : NodeBase
{
    public Mention(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "mention";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
            ["renderLabel"] = (Func<ProseMirrorNode, string?>)(node => null),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "span[data-type=\"mention\"]",
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        return new Dictionary<string, AttributeConfiguration>
        {
            ["id"] = new AttributeConfiguration
            {
                ParseHTML = domNode => domNode is IElement element ? element.GetAttribute("data-id") : null,
                RenderHTML = attrs =>
                {
                    if (attrs is IDictionary<string, object?> dictionary && dictionary.TryGetValue("id", out var value))
                    {
                        return new Dictionary<string, object?>
                        {
                            ["data-id"] = value,
                        };
                    }

                    return null;
                },
            },
        };
    }

    public override string? RenderText(ProseMirrorNode node)
    {
        var renderLabel = this.GetOptionDelegate<Func<ProseMirrorNode, string?>>("renderLabel");
        return renderLabel?.Invoke(node);
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "span",
            HtmlUtilities.MergeAttributes(
                new Dictionary<string, object?>
                {
                    ["data-type"] = "mention",
                },
                this.GetHtmlAttributes()),
            0,
        };
    }
}
