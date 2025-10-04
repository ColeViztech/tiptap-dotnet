using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class TableHeader : TableCell
{
    public TableHeader(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "tableHeader";

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
            Tag = "th",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "th",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
