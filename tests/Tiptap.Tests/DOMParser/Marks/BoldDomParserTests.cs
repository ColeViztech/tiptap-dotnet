using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class BoldDomParserTests
{
    [Fact]
    public void BTagGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><b>Example</b> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("bold")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void StrongTagGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><strong>Example</strong> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("bold")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void BTagWithFontWeightNormalIsIgnored()
    {
        var result = new EditorClass()
            .SetContent("<p><b style=\"font-weight: normal;\">Example</b> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithFontWeightBoldIsParsed()
    {
        var result = new EditorClass()
            .SetContent("<p><span style=\"font-weight: bold;\">Example</span> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("bold")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithFontWeight500IsParsed()
    {
        var result = new EditorClass()
            .SetContent("<p><span style=\"font-weight: 500;\">Example</span> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("bold")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
