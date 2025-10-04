using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class BulletListDomParserTests
{
    [Fact]
    public void BulletListGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<ul><li><p>Example</p></li><li><p>Text</p></li></ul>")
            .GetDocument();

        var expected = Doc(
            BulletList(
                ListItem(
                    Paragraph(
                        Text("Example"))),
                ListItem(
                    Paragraph(
                        Text("Text")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void BulletListItemWithTextOnlyGetsWrappedInParagraph()
    {
        var result = new EditorClass()
            .SetContent("<ul><li><p>Example</p></li><li><p>Text <em>Test</em></p></li></ul>")
            .GetDocument();

        var expected = Doc(
            BulletList(
                ListItem(
                    Paragraph(
                        Text("Example"))),
                ListItem(
                    Paragraph(
                        Text("Text "),
                        Text(
                            "Test",
                            Mark("italic"))))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ListItemsWithSpaceGetRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<ul><li><p> </p></li></ul>")
            .GetDocument();

        var expected = Doc(
            BulletList(
                ListItem(
                    Paragraph())));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ListItemsContentGetRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<ul><li><p>Tiptap</p></li></ul>")
            .GetDocument();

        var expected = Doc(
            BulletList(
                ListItem(
                    Paragraph(
                        Text("Tiptap")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
