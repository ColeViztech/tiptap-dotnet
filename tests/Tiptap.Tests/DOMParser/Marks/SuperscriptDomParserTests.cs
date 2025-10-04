using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class SuperscriptDomParserTests
{
    [Fact]
    public void SuperscriptTagGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Superscript(),
                }))
            .SetContent("<p><sup>Example Text</sup></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("superscript"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithVerticalAlignSuperIsParsed()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Superscript(),
                }))
            .SetContent("<p><span style=\"vertical-align: super;\">Example Text</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("superscript"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
