using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMParser;

public class MultipleMarksDomParserTests
{
    [Fact]
    public void MultipleMarksAreRenderedCorrectly()
    {
        var html = "<p><strong><em>Example Text</em></strong></p>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Example Text",
                    Mark("bold"),
                    Mark("italic"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
