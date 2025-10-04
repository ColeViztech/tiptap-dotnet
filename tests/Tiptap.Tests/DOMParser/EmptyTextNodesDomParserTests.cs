using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMParser;

public class EmptyTextNodesDomParserTests
{
    [Fact]
    public void OutputMustNotHaveEmptyTextNodes()
    {
        var html = "<em><br />\n</em>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            HardBreak(
                Mark("italic")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
