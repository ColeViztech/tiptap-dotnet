using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class CodeDomParserTests
{
    [Fact]
    public void CodeTagGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><code>Example Text</code></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("code"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
