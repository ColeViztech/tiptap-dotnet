using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class SpecialCharacterDomParserTests
{
    [Fact]
    public void EmojisAreTransformedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>🔥</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("🔥")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ExtendedEmojisAreTransformedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>👩‍👩‍👦</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("👩‍👩‍👦")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void UmlautsAreTransformedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>äöüÄÖÜß</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("äöüÄÖÜß")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void HtmlEntitiesAreTransformedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>&lt;</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("<")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
