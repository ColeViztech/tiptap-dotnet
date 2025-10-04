using System.Linq;
using System.Net;
using System.Text;

namespace Tiptap.Core.Utils;

public static class HtmlUtilities
{
    public static IDictionary<string, object?> MergeAttributes(params IDictionary<string, object?>?[] sources)
    {
        var attributes = new Dictionary<string, object?>();

        foreach (var source in sources)
        {
            if (source == null)
            {
                continue;
            }

            foreach (var (key, value) in source)
            {
                switch (key)
                {
                    case "class":
                        var existingClass = attributes.TryGetValue("class", out var classValue)
                            ? classValue?.ToString()
                            : string.Empty;

                        var combinedClass = string.Join(" ", new[] { existingClass, value?.ToString() }
                            .Where(v => !string.IsNullOrWhiteSpace(v)));

                        attributes["class"] = combinedClass.Trim();
                        break;
                    case "style":
                        var existingStyle = attributes.TryGetValue("style", out var styleValue)
                            ? styleValue?.ToString() ?? string.Empty
                            : string.Empty;

                        var styleBuilder = new StringBuilder();
                        if (!string.IsNullOrWhiteSpace(existingStyle))
                        {
                            styleBuilder.Append(existingStyle.Trim().TrimEnd(';'));
                            styleBuilder.Append("; ");
                        }

                        if (!string.IsNullOrWhiteSpace(value?.ToString()))
                        {
                            styleBuilder.Append(value?.ToString()?.Trim().TrimEnd(';'));
                            styleBuilder.Append(';');
                        }

                        var styleResult = styleBuilder.ToString().Trim().Trim(';').Trim();
                        attributes["style"] = string.IsNullOrEmpty(styleResult) ? null : styleResult;
                        break;
                    default:
                        attributes[key] = value;
                        break;
                }
            }
        }

        return attributes;
    }

    public static string RenderAttributes(IDictionary<string, object?> attrs)
    {
        var rendered = new List<string>();

        foreach (var (key, value) in attrs)
        {
            if (value is null)
            {
                continue;
            }

            var normalisedValue = value switch
            {
                bool boolean => boolean ? "true" : "false",
                _ => value.ToString(),
            };

            if (string.IsNullOrEmpty(normalisedValue))
            {
                continue;
            }

            var escapedValue = WebUtility.HtmlEncode(normalisedValue);
            rendered.Add($" {key}=\"{escapedValue}\"");
        }

        return string.Join(string.Empty, rendered);
    }
}
