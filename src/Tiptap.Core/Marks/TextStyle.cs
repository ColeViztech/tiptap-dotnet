using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class TextStyle : Mark
{
    public TextStyle(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "textStyle";

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
            Tag = "span",
            GetAttrs = parameter =>
            {
                if (parameter is IElement element)
                {
                    return element.HasAttribute("style") ? null : false;
                }

                return false;
            },
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        return new object?[]
        {
            "span",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
