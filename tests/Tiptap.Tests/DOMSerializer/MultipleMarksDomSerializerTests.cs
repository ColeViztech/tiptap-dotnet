using Tiptap.Core;
using Tiptap.Tests;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer;

public class MultipleMarksDomSerializerTests
{
    [Fact]
    public void MultipleMarksGetRenderedCorrectly()
    {
        var document = Doc(
            Paragraph(
                Text(
                    "Example Text",
                    Mark("bold"),
                    Mark("italic"))));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><strong><em>Example Text</em></strong></p>", result);
    }

    [Fact]
    public void MultipleMarksGetRenderedCorrectlyWithAdditionalMarkAtFirstNode()
    {
        var document = Doc(
            Text(
                "lorem ",
                Mark("italic"),
                Mark("bold")),
            Text(
                "ipsum",
                Mark("bold")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<em><strong>lorem </strong></em><strong>ipsum</strong>", result);
    }

    [Fact]
    public void MultipleMarksGetRenderedCorrectlyWithAdditionalMarkAtLastNode()
    {
        var document = Doc(
            Text(
                "lorem ",
                Mark("italic")),
            Text(
                "ipsum",
                Mark("italic"),
                Mark("bold")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<em>lorem <strong>ipsum</strong></em>", result);
    }

    [Fact]
    public void MultipleMarksGetRenderedCorrectlyWhenOverlappingMarksExist()
    {
        var document = Doc(
            Paragraph(
                Text(
                    "lorem ",
                    Mark("bold")),
                Text(
                    "ipsum",
                    Mark("bold"),
                    Mark("italic")),
                Text(
                    " dolor",
                    Mark("italic")),
                Text(" sit")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><strong>lorem <em>ipsum</em></strong><em> dolor</em> sit</p>", result);
    }

    [Fact]
    public void MultipleMarksGetRenderedCorrectlyWhenOverlappingPassagesWithMultipleMarksExist()
    {
        var document = Doc(
            Paragraph(
                Text(
                    "lorem ",
                    Mark("bold"),
                    Mark("strike")),
                Text(
                    "ipsum",
                    Mark("italic"),
                    Mark("bold"),
                    Mark("strike")),
                Text(
                    " dolor",
                    Mark("strike"),
                    Mark("italic"))));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><strong><s>lorem <em>ipsum</em></s></strong><s><em> dolor</em></s></p>", result);
    }
}
