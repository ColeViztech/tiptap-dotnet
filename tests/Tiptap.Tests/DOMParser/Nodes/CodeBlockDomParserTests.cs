using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class CodeBlockDomParserTests
{
    [Fact]
    public void CodeBlockGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<pre><code>Example Text</code></pre>")
            .GetDocument();

        var expected = Doc(
            CodeBlock(
                Text(
                    "Example Text",
                    Mark("code"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void CodeBlockWithLanguageGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<pre><code class=\"language-css\">body { display: none }</code></pre>")
            .GetDocument();

        var expected = Doc(
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
    public void LanguageClassPrefixIsConfigurable()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(new Dictionary<string, object?>
                {
                    ["codeBlock"] = new Dictionary<string, object?>
                    {
                        ["languageClassPrefix"] = "custom-language-prefix-",
                    },
                }),
            }));

        var result = editor
            .SetContent("<pre><code class=\"custom-language-prefix-css\">body { display: none }</code></pre>")
            .GetDocument();

        var expected = Doc(
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
    public void CodeBlockAndInlineCodeAreRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><code>Example Text</code></p><pre><code>body { display: none }</code></pre>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Example Text",
                    Mark("code"))),
            CodeBlock(
                Text(
                    "body { display: none }",
                    Mark("code"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void HandlesCodeBlocksWithoutCodeTag()
    {
        var result = new EditorClass()
            .SetContent("<pre>body { display: none }</pre>")
            .GetDocument();

        var expected = Doc(
            CodeBlock(
                Text("body { display: none }")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
