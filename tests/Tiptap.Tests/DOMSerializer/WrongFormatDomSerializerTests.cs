using System;
using System.Collections.Generic;
using System.Text.Json;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using Tiptap.Tests;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer;

public class WrongFormatDomSerializerTests
{
    [Fact]
    public void NodeContentStringThrowsJsonException()
    {
        var document = new Dictionary<string, object?>
        {
            ["type"] = "doc",
            ["content"] = "test",
        };

        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
            }));

        Assert.Throws<JsonException>(() => editor.SetContent(document));
    }

    [Fact]
    public void NodeContentEmptyArrayGetsRenderedAsEmptyString()
    {
        var document = new Dictionary<string, object?>
        {
            ["type"] = "doc",
            ["content"] = Array.Empty<object?>(),
        };

        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
            }));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Empty(result);
    }

    [Fact]
    public void NodeContentContainingEmptyArraysThrowsJsonException()
    {
        var document = new Dictionary<string, object?>
        {
            ["type"] = "doc",
            ["content"] = new object?[]
            {
                Array.Empty<object?>(),
                Array.Empty<object?>(),
            },
        };

        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
            }));

        Assert.Throws<JsonException>(() => editor.SetContent(document));
    }

    [Fact]
    public void NodeContentWithValidNestedNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            CodeBlock(
                Text("Example Text")));

        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
            }));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<pre><code>Example Text</code></pre>", result);
    }

    [Fact]
    public void NodeContentWithValidMarkGetsRenderedCorrectly()
    {
        var document = Doc(
            Text(
                "Example Link",
                Mark("link", new Dictionary<string, object?>
                {
                    ["href"] = "https://tiptap.dev",
                })));

        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var result = editor
            .SetContent(document)
            .GetHTML();

        const string expected = "<a href=\"https://tiptap.dev\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">Example Link</a>";
        Assert.Equal(expected, result);
    }
}
