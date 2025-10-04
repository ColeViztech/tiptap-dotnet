using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace Tiptap.Core.Utils;

public static class InlineStyle
{
    private static readonly Regex StyleRegex = new("([\\w-]+)\\s*:\\s*([^;]+)\\s*;?", RegexOptions.Compiled);

    public static IDictionary<string, string> Get(INode node)
    {
        if (node is not IElement element)
        {
            return new Dictionary<string, string>();
        }

        var style = element.GetAttribute("style");
        if (string.IsNullOrEmpty(style))
        {
            return new Dictionary<string, string>();
        }

        var matches = StyleRegex.Matches(style);
        return matches
            .Cast<Match>()
            .Where(match => match.Groups.Count >= 3)
            .ToDictionary(match => match.Groups[1].Value, match => match.Groups[2].Value);
    }

    public static bool HasAttribute(INode node, object value)
    {
        var styles = Get(node);

        return value switch
        {
            IDictionary<string, string> map => map.All(pair => styles.TryGetValue(pair.Key, out var styleValue) && styleValue == pair.Value),
            IEnumerable<KeyValuePair<string, string>> pairs => pairs.All(pair => styles.TryGetValue(pair.Key, out var styleValue) && styleValue == pair.Value),
            string key => styles.ContainsKey(key),
            _ => throw new InvalidOperationException($"Can't compare inline styles to {value}"),
        };
    }

    public static string? GetAttribute(INode node, string attribute)
    {
        var styles = Get(node);
        return styles.TryGetValue(attribute, out var value) ? value : null;
    }
}
