using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class NestedMarksDomParserTests
{
    [Fact]
    public void NestedMarksAreRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<strong>only bold <em>bold and italic</em> only bold</strong>")
            .GetDocument();

        var expected = Doc(
            Text("only bold ", Mark("bold")),
            Text("bold and italic", Mark("bold"), Mark("italic")),
            Text(" only bold", Mark("bold")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
