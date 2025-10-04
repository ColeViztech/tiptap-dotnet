using System.Collections.Generic;
using Tiptap.Tests;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer;

public class ExampleJsonDomSerializerTests
{
    [Fact]
    public void ExampleJsonGetsRenderedCorrectly()
    {
        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 2,
                },
                Text("Export HTML or JSON")),
            Paragraph(
                Text("You are able to export your data as "),
                Text("HTML", Mark("code")),
                Text(" or "),
                Text("JSON", Mark("code")),
                Text(". To pass "),
                Text("HTML", Mark("code")),
                Text(" to the editor use the "),
                Text("content", Mark("code")),
                Text(" slot. To pass "),
                Text("JSON", Mark("code")),
                Text(" to the editor use the "),
                Text("doc", Mark("code")),
                Text(" prop.")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        const string expected = "<h2>Export HTML or JSON</h2><p>You are able to export your data as <code>HTML</code> or <code>JSON</code>. To pass <code>HTML</code> to the editor use the <code>content</code> slot. To pass <code>JSON</code> to the editor use the <code>doc</code> prop.</p>";

        Assert.Equal(expected, result);
    }
}
