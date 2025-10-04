using Tiptap.Core.Models;

namespace Tiptap.Core.Core;

public class JsonSerializer
{
    private readonly System.Text.Json.JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    public string Process(object value)
    {
        var document = Serialization.ToDocument(value);
        return System.Text.Json.JsonSerializer.Serialize(document, _options);
    }
}
