using System.Linq;
using System.Text.Json.Serialization;

namespace Tiptap.Core.Models;

public class ProseMirrorNode
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("attrs")]
    public Dictionary<string, object?>? Attrs { get; set; }

    [JsonPropertyName("content")]
    public List<ProseMirrorNode>? Content { get; set; }

    [JsonPropertyName("marks")]
    public List<ProseMirrorMark>? Marks { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object?>? AdditionalData { get; set; }

    public ProseMirrorNode Clone()
    {
        return new ProseMirrorNode
        {
            Type = Type,
            Text = Text,
            Attrs = Attrs != null
                ? new Dictionary<string, object?>(Attrs)
                : null,
            Content = Content?.Select(node => node.Clone()).ToList(),
            Marks = Marks?.Select(mark => mark.Clone()).ToList(),
            AdditionalData = AdditionalData != null
                ? new Dictionary<string, object?>(AdditionalData)
                : null,
        };
    }
}
