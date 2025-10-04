using AngleSharp.Dom;
using Tiptap.Core.Core;
using NodeBase = Tiptap.Core.Core.Node;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class Details : NodeBase
{
    public Details(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "details";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["persist"] = false,
            ["openClassName"] = "is-open",
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "details",
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        if (!this.GetBoolOption("persist"))
        {
            return new Dictionary<string, AttributeConfiguration>();
        }

        return new Dictionary<string, AttributeConfiguration>
        {
            ["open"] = new AttributeConfiguration
            {
                Default = false,
                ParseHTML = domNode => domNode is IElement element && element.HasAttribute("open"),
                RenderHTML = attrs =>
                {
                    if (attrs is IDictionary<string, object?> dictionary && dictionary.TryGetValue("open", out var value))
                    {
                        var isOpen = value switch
                        {
                            bool boolean => boolean,
                            string text when bool.TryParse(text, out var parsed) => parsed,
                            int number => number != 0,
                            _ => false,
                        };

                        if (isOpen)
                        {
                            return new Dictionary<string, object?>
                            {
                                ["open"] = "open",
                            };
                        }
                    }

                    return new Dictionary<string, object?>();
                },
            },
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        var attributes = this.GetHtmlAttributes();

        if (this.GetBoolOption("persist") && node.Attrs != null && node.Attrs.TryGetValue("open", out var value))
        {
            var isOpen = value switch
            {
                bool boolean => boolean,
                string text when bool.TryParse(text, out var parsed) => parsed,
                int number => number != 0,
                _ => false,
            };

            if (isOpen)
            {
                var openClassName = this.GetStringOption("openClassName", "is-open");
                attributes = HtmlUtilities.MergeAttributes(
                    attributes,
                    new Dictionary<string, object?>
                    {
                        ["class"] = openClassName,
                    });
            }
        }

        return new object?[]
        {
            "details",
            attributes,
            0,
        };
    }
}
