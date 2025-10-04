using AngleSharp.Dom;
using Tiptap.Core.Core;
using NodeBase = Tiptap.Core.Core.Node;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class DetailsContent : NodeBase
{
    public DetailsContent(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "detailsContent";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "div[data-type]",
            GetAttrs = domNode =>
            {
                if (domNode is IElement element && element.GetAttribute("data-type") == "detailsContent")
                {
                    return new Dictionary<string, object?>();
                }

                return false;
            },
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "div",
            HtmlUtilities.MergeAttributes(
                this.GetHtmlAttributes(),
                new Dictionary<string, object?>
                {
                    ["data-type"] = "detailsContent",
                }),
            0,
        };
    }
}
