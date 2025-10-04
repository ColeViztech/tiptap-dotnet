using System.Linq;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Extensions;

public class Color : Extension
{
    public Color(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "color";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["types"] = new List<string> { "textStyle" },
        };
    }

    private IReadOnlyList<string> GetTypes()
    {
        if (Options.TryGetValue("types", out var value))
        {
            switch (value)
            {
                case IEnumerable<string> stringEnumerable:
                    return stringEnumerable.ToList();
                case IEnumerable<object?> objectEnumerable:
                    return objectEnumerable
                        .Select(item => item?.ToString())
                        .Where(text => !string.IsNullOrEmpty(text))
                        .Select(text => text!)
                        .ToList();
            }
        }

        return new List<string> { "textStyle" };
    }

    public override IEnumerable<GlobalAttributeConfiguration> AddGlobalAttributes()
    {
        yield return new GlobalAttributeConfiguration(
            GetTypes(),
            new Dictionary<string, AttributeConfiguration>
            {
                ["color"] = new AttributeConfiguration
                {
                    Default = null,
                    ParseHTML = node =>
                    {
                        if (node is INode domNode)
                        {
                            var attribute = InlineStyle.GetAttribute(domNode, "color");
                            if (attribute == null)
                            {
                                return null;
                            }

                            return attribute.Replace("\"", string.Empty).Replace("'", string.Empty);
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
                                    ["style"] = $"color: {color}",
                                };
                            }
                        }

                        return null;
                    },
                },
            });
    }
}
