using AngleSharp.Dom;
using Tiptap.Core.Core;
using NodeBase = Tiptap.Core.Core.Node;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class OrderedList : NodeBase
{
    public OrderedList(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "orderedList";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        return new Dictionary<string, AttributeConfiguration>
        {
            ["start"] = new AttributeConfiguration
            {
                ParseHTML = domNode => domNode is IElement element && int.TryParse(element.GetAttribute("start"), out var start)
                    ? start
                    : null,
                RenderHTML = attrs =>
                {
                    if (attrs is IDictionary<string, object?> dictionary && dictionary.TryGetValue("start", out var value) && value != null)
                    {
                        return new Dictionary<string, object?>
                        {
                            ["start"] = value,
                        };
                    }

                    return null;
                },
            },
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "ol",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "ol",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
