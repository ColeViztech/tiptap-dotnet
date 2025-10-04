using Tiptap.Core.Core;
using Tiptap.Core.Extensions;

namespace Tiptap.Tests.SchemaTests;

public class PriorityTests
{
    [Fact]
    public void ParagraphIsTheDefaultNode()
    {
        var schema = new Schema(new[]
        {
            new StarterKit(),
        });

        Assert.Equal("paragraph", schema.DefaultNode?.Name);
    }
}
