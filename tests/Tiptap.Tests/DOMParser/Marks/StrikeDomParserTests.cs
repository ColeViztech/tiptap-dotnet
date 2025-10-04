using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class StrikeDomParserTests
{
    [Fact]
    public void StrikeVariantsAreRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><strike>Example text using strike</strike> and <s>example text using s</s> and <del>example text using del</del></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example text using strike", Mark("strike")),
                Text(" and "),
                Text("example text using s", Mark("strike")),
                Text(" and "),
                Text("example text using del", Mark("strike"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithLineThroughStyleIsParsed()
    {
        var result = new EditorClass()
            .SetContent("<p><span style=\"text-decoration: line-through\">Example Text</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("strike"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
