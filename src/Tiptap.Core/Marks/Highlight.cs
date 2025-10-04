using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class Highlight : Mark
{
    public Highlight(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "highlight";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["multicolor"] = false,
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "mark",
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        if (!this.GetBoolOption("multicolor"))
        {
            return new Dictionary<string, AttributeConfiguration>();
        }

        return new Dictionary<string, AttributeConfiguration>
        {
            ["color"] = new AttributeConfiguration
            {
                ParseHTML = node =>
                {
                    if (node is IElement element)
                    {
                        var dataColor = element.GetAttribute("data-color");
                        if (!string.IsNullOrEmpty(dataColor))
                        {
                            return dataColor;
                        }
                    }

                    if (node is INode domNode)
                    {
                        var attribute = InlineStyle.GetAttribute(domNode, "background-color");
                        if (!string.IsNullOrEmpty(attribute))
                        {
                            return attribute.Replace("\"", string.Empty).Replace("'", string.Empty);
                        }
                    }

                    return null;
                },
                RenderHTML = attributes =>
                {
                    if (attributes is IDictionary<string, object?> map && map.TryGetValue("color", out var value))
                    {
                        var color = value?.ToString();
                        if (!string.IsNullOrEmpty(color))
                        {
                            return new Dictionary<string, object?>
                            {
                                ["data-color"] = color,
                                ["style"] = $"background-color: {color}",
                            };
                        }
                    }

                    return null;
                },
            },
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        return new object?[]
        {
            "mark",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
