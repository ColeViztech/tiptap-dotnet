using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class Underline : Mark
{
    public Underline(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "underline";

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
            Tag = "u",
        };

        yield return new ParseRule
        {
            Style = "text-decoration=underline",
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        return new object?[]
        {
            "u",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
