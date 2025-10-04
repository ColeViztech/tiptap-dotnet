using System.Text.Json.Serialization;

namespace Tiptap.Core.Models;

public class ProseMirrorMark
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("attrs")]
    public Dictionary<string, object?>? Attrs { get; set; }

    public ProseMirrorMark Clone()
    {
        return new ProseMirrorMark
        {
            Type = Type,
            Attrs = Attrs != null
                ? new Dictionary<string, object?>(Attrs)
                : null,
        };
    }
}
