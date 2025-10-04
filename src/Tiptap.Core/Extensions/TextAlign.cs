using System.Linq;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Extensions;

public class TextAlign : Extension
{
    public TextAlign(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "textAlign";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["types"] = new List<string>(),
            ["alignments"] = new List<string> { "left", "center", "right", "justify" },
            ["defaultAlignment"] = "left",
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

        return new List<string>();
    }

    private IReadOnlyList<string> GetAlignments()
    {
        if (Options.TryGetValue("alignments", out var value))
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

        return new List<string> { "left", "center", "right", "justify" };
    }

    private string GetDefaultAlignment()
    {
        var alignments = GetAlignments();
        if (Options.TryGetValue("defaultAlignment", out var value))
        {
            var text = value?.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                return alignments.Contains(text)
                    ? text
                    : alignments.FirstOrDefault() ?? "left";
            }
        }

        return alignments.FirstOrDefault() ?? "left";
    }

    public override IEnumerable<GlobalAttributeConfiguration> AddGlobalAttributes()
    {
        var defaultAlignment = GetDefaultAlignment();
        var alignments = GetAlignments();

        yield return new GlobalAttributeConfiguration(
            GetTypes(),
            new Dictionary<string, AttributeConfiguration>
            {
                ["textAlign"] = new AttributeConfiguration
                {
                    Default = defaultAlignment,
                    ParseHTML = node =>
                    {
                        if (node is INode domNode)
                        {
                            return InlineStyle.GetAttribute(domNode, "text-align") ?? defaultAlignment;
                        }

                        return defaultAlignment;
                    },
                    RenderHTML = attributes =>
                    {
                        if (attributes is IDictionary<string, object?> map && map.TryGetValue("textAlign", out var value))
                        {
                            var alignment = value?.ToString();
                            if (!string.IsNullOrEmpty(alignment) && !string.Equals(alignment, defaultAlignment, StringComparison.OrdinalIgnoreCase))
                            {
                                if (alignments.Count == 0 || alignments.Contains(alignment))
                                {
                                    return new Dictionary<string, object?>
                                    {
                                        ["style"] = $"text-align: {alignment}",
                                    };
                                }
                            }
                        }

                        return null;
                    },
                },
            });
    }
}
