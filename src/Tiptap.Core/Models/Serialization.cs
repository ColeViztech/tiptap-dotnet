using System.Text.Json;

namespace Tiptap.Core.Models;

public static class Serialization
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    public static ProseMirrorDocument ToDocument(object value)
    {
        if (value is ProseMirrorDocument document)
        {
            return document;
        }

        var json = JsonSerializer.Serialize(value, Options);
        return JsonSerializer.Deserialize<ProseMirrorDocument>(json, Options)
               ?? new ProseMirrorDocument();
    }

    public static object FromDocument(ProseMirrorDocument document)
    {
        return JsonSerializer.Deserialize<object>(
            JsonSerializer.Serialize(document, Options),
            Options
        ) ?? new object();
    }
}
