using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using Tiptap.Tests;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class CodeBlockShikiDomSerializerTests
{
    private static EditorClass CreateEditor(Extension codeBlockExtension)
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(new Dictionary<string, object?>
                {
                    ["codeBlock"] = false,
                }),
                codeBlockExtension,
            }));
    }

    [Fact]
    public void EditorCanBeCreatedWithCodeBlockShikiExtension()
    {
        var editor = CreateEditor(new CodeBlockShiki());

        var extension = Assert.IsType<CodeBlockShiki>(editor.Extensions[1]);
        Assert.Equal("nord", extension.Options["theme"]);
    }

    [Fact]
    public void DefaultThemeCanBeConfigured()
    {
        var editor = CreateEditor(new CodeBlockShiki(new Dictionary<string, object?>
        {
            ["theme"] = "mojave",
        }));

        var extension = Assert.IsType<CodeBlockShiki>(editor.Extensions[1]);
        Assert.Equal("mojave", extension.Options["theme"]);
    }

    [Fact]
    public void DefaultLanguageCanBeConfigured()
    {
        var editor = CreateEditor(new CodeBlockShiki(new Dictionary<string, object?>
        {
            ["defaultLanguage"] = "css",
        }));

        var extension = Assert.IsType<CodeBlockShiki>(editor.Extensions[1]);
        Assert.Equal("css", extension.Options["defaultLanguage"]);
    }

    [Fact]
    public void CodeBlockAndInlineCodeAreRenderedCorrectly()
    {
        var editor = CreateEditor(new CodeBlockShiki(new Dictionary<string, object?>
        {
            ["defaultLanguage"] = "css",
        }));

        var result = editor
            .SetContent("<p><code>Example Text</code></p><pre><code class=\"language-css\">body { display: none }</code></pre>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Example Text",
                    Mark("code"))),
            CodeBlock(
                new Dictionary<string, object?>
                {
                    ["language"] = "css",
                },
                Text(
                    "body { display: none }",
                    Mark("code"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void HtmlResultIsRenderedWithShikiHighlighting()
    {
        var editor = CreateEditor(new CodeBlockShiki(new Dictionary<string, object?>
        {
            ["defaultLanguage"] = "css",
        }));

        var result = editor
            .SetContent("<p><code>Example Text</code></p><pre><code class=\"language-css\">body { display: none }</code></pre>")
            .GetHTML();

        const string expected = "<p><code>Example Text</code></p><pre><code class=\"language-css\">body { display: none }</code></pre><code>body { display: none }</code>";

        Assert.Equal(expected, result);
    }
}
