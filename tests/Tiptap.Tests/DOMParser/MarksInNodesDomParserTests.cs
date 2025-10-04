using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class MarksInNodesDomParserTests
{
    [Fact]
    public void ParagraphWithMarksGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var result = editor
            .SetContent("<p>Example <strong><em>Text</em></strong>.</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example "),
                Text(
                    "Text",
                    Mark("bold"),
                    Mark("italic")),
                Text(".")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ComplexMarkupGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var html = """
<h1>Headline 1</h1>
<p>
    Some text. <strong>Bold Text</strong>. <em>Italic Text</em>. <strong><em>Bold and italic Text</em></strong>. Here is a <a href="https://tiptap.dev">Link</a>.
</p>
""";

        var result = editor
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                },
                Text("Headline 1")),
            Paragraph(
                Text("Some text. "),
                Text(
                    "Bold Text",
                    Mark("bold")),
                Text(". "),
                Text(
                    "Italic Text",
                    Mark("italic")),
                Text(". "),
                Text(
                    "Bold and italic Text",
                    Mark("bold"),
                    Mark("italic")),
                Text(". Here is a "),
                Text(
                    "Link",
                    Mark(
                        "link",
                        new Dictionary<string, object?>
                        {
                            ["href"] = "https://tiptap.dev",
                        })),
                Text(".")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void MultipleListsGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var html = """
<h2>Headline 2</h2>
<ol>
    <li><p>ordered list item</p></li>
    <li><p>ordered list item</p></li>
    <li><p>ordered list item</p></li>
</ol>
<ul>
    <li><p>unordered list item</p></li>
    <li><p>unordered list item with <a href="https://tiptap.dev"><strong>link</strong></a></p></li>
    <li><p>unordered list item</p></li>
</ul>
<p>Some Text.</p>
""";

        var result = editor
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 2,
                },
                Text("Headline 2")),
            OrderedList(
                ListItem(
                    Paragraph(
                        Text("ordered list item"))),
                ListItem(
                    Paragraph(
                        Text("ordered list item"))),
                ListItem(
                    Paragraph(
                        Text("ordered list item")))),
            BulletList(
                ListItem(
                    Paragraph(
                        Text("unordered list item"))),
                ListItem(
                    Paragraph(
                        Text("unordered list item with "),
                        Text(
                            "link",
                            Mark(
                                "link",
                                new Dictionary<string, object?>
                                {
                                    ["href"] = "https://tiptap.dev",
                                }),
                            Mark("bold")))),
                ListItem(
                    Paragraph(
                        Text("unordered list item")))),
            Paragraph(
                Text("Some Text.")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
