using Tiptap.Core.Core;

namespace Tiptap.Core.Marks;

internal static class MarkOptionExtensions
{
    public static IDictionary<string, object?> GetHtmlAttributes(this Mark mark)
    {
        if (mark.Options.TryGetValue("HTMLAttributes", out var value) && value is IDictionary<string, object?> dictionary)
        {
            return dictionary;
        }

        return new Dictionary<string, object?>();
    }

    public static bool GetBoolOption(this Mark mark, string key, bool fallback = false)
    {
        if (mark.Options.TryGetValue(key, out var value))
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
}
