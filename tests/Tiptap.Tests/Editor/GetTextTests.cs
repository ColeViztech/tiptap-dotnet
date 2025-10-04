using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class GetTextTests
{
    [Fact]
    public void GetTextReturnsPlainText()
    {
        var text = new EditorClass()
            .SetContent("<h1>Heading</h1><p>Paragraph</p>")
            .GetText();

        Assert.Equal("Heading\n\nParagraph", text);
    }

    [Fact]
    public void GetTextOnlyReturnsOneBlockSeparatorBetweenBlocks()
    {
        var text = new EditorClass()
            .SetContent("<h1>Heading</h1><p>Paragraph</p><ul><li><p>ListItem</p></li></ul>")
            .GetText();

        Assert.Equal("Heading\n\nParagraph\n\nListItem", text);
    }

    [Fact]
    public void GetTextAllowsConfiguringTheBlockSeparator()
    {
        var text = new EditorClass()
            .SetContent("<h1>Heading</h1><p>Paragraph</p>")
            .GetText(new Dictionary<string, object?>
            {
                ["blockSeparator"] = "\n",
            });

        Assert.Equal("Heading\nParagraph", text);
    }
}
