using Tiptap.Core.Core;

namespace Tiptap.Core.Nodes;

internal static class NodeOptionExtensions
{
    public static IDictionary<string, object?> GetHtmlAttributes(this Node node)
    {
        if (node.Options.TryGetValue("HTMLAttributes", out var value) && value is IDictionary<string, object?> dictionary)
        {
            return dictionary;
        }

        return new Dictionary<string, object?>();
    }

    public static string GetStringOption(this Node node, string key, string fallback)
    {
        if (node.Options.TryGetValue(key, out var value) && value is string stringValue)
        {
            return stringValue;
        }

        return fallback;
    }

    public static bool GetBoolOption(this Node node, string key, bool fallback = false)
    {
        if (node.Options.TryGetValue(key, out var value))
        {
            return value switch
            {
                bool boolean => boolean,
                string text when bool.TryParse(text, out var parsed) => parsed,
                int number => number != 0,
                _ => fallback,
            };
        }

        return fallback;
    }

    public static TDelegate? GetOptionDelegate<TDelegate>(this Node node, string key)
        where TDelegate : class
    {
        if (node.Options.TryGetValue(key, out var value) && value is TDelegate del)
        {
            return del;
        }

        return null;
    }
}
