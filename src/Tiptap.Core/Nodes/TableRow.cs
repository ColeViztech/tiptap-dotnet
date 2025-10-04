using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class TableRow : Node
{
    public TableRow(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "tableRow";

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
            Tag = "tr",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "tr",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
