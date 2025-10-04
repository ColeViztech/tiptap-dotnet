using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class WhitespaceDomParserTests
{
    [Fact]
    public void WhitespaceAtTheBeginningIsStripped()
    {
        var html = "<p>\nExample\n Text</p>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example\nText")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void WhitespaceInCodeBlocksIsIgnored()
    {
        var html = "<p>\n" +
                   "    Example Text\n" +
                   "</p>\n" +
                   "<pre><code>\n" +
                   "Line of Code\n" +
                   "    Line of Code 2\n" +
                   "Line of Code</code></pre>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text")),
            CodeBlock(
                Text(
                    "Line of Code\n    Line of Code 2\nLine of Code",
                    Mark("code"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
