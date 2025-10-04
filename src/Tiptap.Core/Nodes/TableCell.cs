using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using NodeBase = Tiptap.Core.Core.Node;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class TableCell : NodeBase
{
    public TableCell(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "tableCell";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        return new Dictionary<string, AttributeConfiguration>
        {
            ["rowspan"] = new AttributeConfiguration
            {
                ParseHTML = domNode => domNode is IElement element && int.TryParse(element.GetAttribute("rowspan"), out var rowspan)
                    ? rowspan
                    : null,
            },
            ["colspan"] = new AttributeConfiguration
            {
                ParseHTML = domNode => domNode is IElement element && int.TryParse(element.GetAttribute("colspan"), out var colspan)
                    ? colspan
                    : null,
            },
            ["colwidth"] = new AttributeConfiguration
            {
                ParseHTML = domNode =>
                {
                    if (domNode is not IElement element)
                    {
                        return null;
                    }

                    var value = element.GetAttribute("data-colwidth");
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return null;
                    }

                    var widths = value
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(part => int.TryParse(part, out var number) ? number : (int?)null)
                        .Where(number => number.HasValue)
                        .Select(number => number!.Value)
                        .ToArray();

                    return widths.Length > 0 ? widths : null;
                },
                RenderHTML = attrs =>
                {
                    if (attrs is IDictionary<string, object?> dictionary && dictionary.TryGetValue("colwidth", out var value) && value is IEnumerable<object?> enumerable)
                    {
                        var widths = enumerable
                            .Select(item => item switch
                            {
                                int number => number.ToString(CultureInfo.InvariantCulture),
                                string text => text,
                                _ => null,
                            })
                            .Where(text => !string.IsNullOrWhiteSpace(text))
                            .ToArray();

                        if (widths.Length > 0)
                        {
                            return new Dictionary<string, object?>
                            {
                                ["data-colwidth"] = string.Join(",", widths),
                            };
                        }
                    }

                    return null;
                },
            },
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "td",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "td",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
