using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class Image : Node
{
    public Image(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "image";

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
            ["src"] = new AttributeConfiguration(),
            ["alt"] = new AttributeConfiguration(),
            ["title"] = new AttributeConfiguration(),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "img[src]",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "img",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
