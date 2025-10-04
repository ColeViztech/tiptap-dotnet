using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class Bold : Mark
{
    private static readonly Regex FontWeightRegex = new("^(bold(er)?|[5-9]\\d{2,})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Bold(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "bold";

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
            Tag = "strong",
        };

        yield return new ParseRule
        {
            Tag = "b",
            GetAttrs = parameter =>
            {
                if (parameter is INode node && !InlineStyle.HasAttribute(node, new Dictionary<string, string>
                    {
                        ["font-weight"] = "normal",
                    }))
                {
                    return null;
                }

                return false;
            },
        };

        yield return new ParseRule
        {
            Style = "font-weight",
            GetAttrs = parameter =>
            {
                var value = parameter?.ToString();
                if (value != null && FontWeightRegex.IsMatch(value))
                {
                    return null;
                }

                return false;
            },
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        return new object?[]
        {
            "strong",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
