using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class Table : Node
{
    public Table(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "table";

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
            Tag = "table",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "table",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            new object?[]
            {
                "tbody",
                0,
            },
        };
    }
}
