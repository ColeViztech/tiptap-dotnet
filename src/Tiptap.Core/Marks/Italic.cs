using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class Italic : Mark
{
    public Italic(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "italic";

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
            Tag = "em",
        };

        yield return new ParseRule
        {
            Tag = "i",
            GetAttrs = parameter =>
            {
                if (parameter is INode node && !InlineStyle.HasAttribute(node, new Dictionary<string, string>
                    {
                        ["font-style"] = "normal",
                    }))
                {
                    return null;
                }

                return false;
            },
        };

        yield return new ParseRule
        {
            Style = "font-style=italic",
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        return new object?[]
        {
            "em",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
