using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tiptap.Tests;

internal static class JsonTestHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, Options);
    }
}
