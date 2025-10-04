using System.Linq;
using System.Text.Json.Serialization;

namespace Tiptap.Core.Models;

public class ProseMirrorDocument
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("content")]
    public List<ProseMirrorNode>? Content { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object?>? AdditionalData { get; set; }

    public ProseMirrorDocument Clone()
    {
        return new ProseMirrorDocument
        {
            Type = Type,
            Content = Content?.Select(node => node.Clone()).ToList(),
            AdditionalData = AdditionalData != null
                ? new Dictionary<string, object?>(AdditionalData)
                : null,
        };
    }
}
