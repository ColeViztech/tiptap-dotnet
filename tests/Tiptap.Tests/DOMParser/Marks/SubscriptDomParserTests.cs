using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class SubscriptDomParserTests
{
    [Fact]
    public void SubscriptTagGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Subscript(),
                }))
            .SetContent("<p><sub>Example Text</sub></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("subscript"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithVerticalAlignSubIsParsed()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Subscript(),
                }))
            .SetContent("<p><span style=\"vertical-align: sub;\">Example Text</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("subscript"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
