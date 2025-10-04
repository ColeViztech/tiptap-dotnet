using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class Strike : Mark
{
    public Strike(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "strike";

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
            Tag = "s",
        };

        yield return new ParseRule
        {
            Tag = "del",
        };

        yield return new ParseRule
        {
            Tag = "strike",
        };

        yield return new ParseRule
        {
            Style = "text-decoration=line-through",
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        return new object?[]
        {
            "s",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
