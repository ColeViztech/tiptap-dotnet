using Tiptap.Core.Core;
using Tiptap.Core.Extensions;

namespace Tiptap.Tests.SchemaTests;

public class GetTopNodeTests
{
    [Fact]
    public void DocumentIsTheTopNode()
    {
        var schema = new Schema(new[]
        {
            new StarterKit(),
        });

        Assert.Equal("doc", schema.TopNode?.Name);
    }
}
