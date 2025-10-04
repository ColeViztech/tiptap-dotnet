using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class DetailsSummary : Node
{
    public DetailsSummary(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "detailsSummary";

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
            Tag = "summary",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "summary",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
