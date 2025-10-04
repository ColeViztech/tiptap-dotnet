using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMParser;

public class KeepContentOfUnknownTagsDomParserTests
{
    [Fact]
    public void KeepsContentOfUnknownTags()
    {
        var html = "<p>Example <x-unknown-tag>Text</x-unknown-tag></p>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void KeepsContentOfUnknownTagsEvenWithKnownMarks()
    {
        var html = "<p>Example <x-unknown-tag><b>Text</b></x-unknown-tag></p>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example "),
                Text(
                    "Text",
                    Mark("bold"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
