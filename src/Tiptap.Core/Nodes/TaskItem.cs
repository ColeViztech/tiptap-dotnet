using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class TaskItem : Node
{
    public TaskItem(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "taskItem";

    public override int Priority => 1000;

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
            ["checked"] = new AttributeConfiguration
            {
                Default = false,
                RenderHTML = attrs =>
                {
                    if (attrs is IDictionary<string, object?> dictionary && dictionary.TryGetValue("checked", out var value))
                    {
                        return new Dictionary<string, object?>
                        {
                            ["data-checked"] = value,
                        };
                    }

                    return new Dictionary<string, object?>
                    {
                        ["data-checked"] = null,
                    };
                },
            },
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "li[data-type=\"taskItem\"]",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        var isChecked = false;
        if (node.Attrs != null && node.Attrs.TryGetValue("checked", out var checkedValue))
        {
            isChecked = checkedValue switch
            {
                bool boolean => boolean,
                string stringValue when bool.TryParse(stringValue, out var parsed) => parsed,
                int intValue => intValue != 0,
                _ => isChecked,
            };
        }

        var inputAttributes = new Dictionary<string, object?>
        {
            ["type"] = "checkbox",
            ["checked"] = isChecked ? "checked" : null,
        };

        return new object?[]
        {
            "li",
            HtmlUtilities.MergeAttributes(
                this.GetHtmlAttributes(),
                new Dictionary<string, object?>
                {
                    ["data-type"] = "taskItem",
                }),
            new object?[]
            {
                "label",
                new object?[]
                {
                    "input",
                    inputAttributes,
                    0,
                },
                new object?[]
                {
                    "span",
                    0,
                },
            },
            new object?[]
            {
                "div",
                0,
            },
        };
    }
}
