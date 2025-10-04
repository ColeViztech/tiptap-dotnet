using System.Linq;
using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class Heading : Node
{
    public Heading(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "heading";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["levels"] = new List<int> { 1, 2, 3, 4, 5, 6 },
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    private IReadOnlyList<int> GetLevels()
    {
        if (Options.TryGetValue("levels", out var value))
        {
            switch (value)
            {
                case IEnumerable<int> intEnumerable:
                    return intEnumerable.ToList();
                case IEnumerable<object?> objectEnumerable:
                    return objectEnumerable
                        .Select(item => item is int number
                            ? number
                            : int.TryParse(item?.ToString(), out var parsed)
                                ? parsed
                                : (int?)null)
                        .Where(number => number.HasValue)
                        .Select(number => number!.Value)
                        .ToList();
            }
        }

        return new List<int> { 1, 2, 3, 4, 5, 6 };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        foreach (var level in GetLevels())
        {
            yield return new ParseRule
            {
                Tag = $"h{level}",
                Attrs = new Dictionary<string, object?>
                {
                    ["level"] = level,
                },
            };
        }
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        var levels = GetLevels();
        var level = levels.FirstOrDefault();

        if (node.Attrs != null && node.Attrs.TryGetValue("level", out var value))
        {
            var parsed = value switch
            {
                int number => number,
                string text when int.TryParse(text, out var number) => number,
                _ => (int?)null,
            };

            if (parsed.HasValue && levels.Contains(parsed.Value))
            {
                level = parsed.Value;
            }
        }

        return new object?[]
        {
            $"h{level}",
            HtmlUtilities.MergeAttributes(this.GetHtmlAttributes()),
            0,
        };
    }
}
