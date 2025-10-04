using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using Tiptap.Tests;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer;

public class InputDomSerializerTests
{
    [Fact]
    public void ArrayGetsRenderedToHtml()
    {
        var document = Doc(
            Text("Example Text"));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("Example Text", result);
    }

    [Fact]
    public void JsonGetsRenderedToHtml()
    {
        var document = Doc(
            Text("Example Text"));

        var json = JsonTestHelper.Serialize(document);

        var result = new EditorClass()
            .SetContent(json)
            .GetHTML();

        Assert.Equal("Example Text", result);
    }

    [Fact]
    public void EncodingIsCorrect()
    {
        var document = Doc(
            Text("Äffchen"));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("Äffchen", result);
    }

    [Fact]
    public void QuotesAreNotEscaped()
    {
        var document = Doc(
            Text("\"Example Text\""));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("\"Example Text\"", result);
    }

    [Fact]
    public void AttributeValuesAreEscaped()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Image(),
            }));

        var document = Doc(
            Image(new Dictionary<string, object?>
            {
                ["src"] = "\"><script type=\"text/javascript\">alert(1);</script><img src=\"",
            }));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<img src=\"&quot;&gt;&lt;script type=&quot;text/javascript&quot;&gt;alert(1);&lt;/script&gt;&lt;img src=&quot;\">", result);
    }

    [Fact]
    public void UnreasonableAttributeNamesAreRemoved()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Image(),
            }));

        var document = Doc(
            Image(new Dictionary<string, object?>
            {
                ["onerror"] = "alert(1)",
                ["src"] = string.Empty,
            }));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<img>", result);
    }
}
