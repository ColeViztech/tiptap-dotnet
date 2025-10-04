using System.Linq;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Extensions;

public class FontFamily : Extension
{
    public FontFamily(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "fontFamily";

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
                ["fontFamily"] = new AttributeConfiguration
                {
                    Default = null,
                    ParseHTML = node =>
                    {
                        if (node is INode domNode)
                        {
                            return InlineStyle.GetAttribute(domNode, "font-family");
                        }

                        return null;
                    },
                    RenderHTML = attributes =>
                    {
                        if (attributes is IDictionary<string, object?> map && map.TryGetValue("fontFamily", out var value))
                        {
                            var fontFamily = value?.ToString();
                            if (!string.IsNullOrEmpty(fontFamily))
                            {
                                return new Dictionary<string, object?>
                                {
                                    ["style"] = $"font-family: {fontFamily}",
                                };
                            }
                        }

                        return null;
                    },
                },
            });
    }
}
