namespace Tiptap.Core.Core;

public class ParseRule
{
    public string? Tag { get; init; }

    public string? Style { get; init; }

    public Func<object?, object?>? GetAttrs { get; init; }

    public IDictionary<string, object?>? Attrs { get; init; }

    public int Priority { get; init; } = 50;

    public Extension? Class { get; set; }
}
