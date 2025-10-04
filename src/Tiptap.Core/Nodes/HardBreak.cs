using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class HardBreak : Node
{
    public HardBreak(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "hardBreak";

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
            Tag = "br",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "br",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
        };
    }
}
