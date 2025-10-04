using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class CodeBlockDomSerializerTests
{
    [Fact]
    public void CodeBlockNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            CodeBlock(
                Text("Example Text")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<pre><code>Example Text</code></pre>", result);
    }

    [Fact]
    public void CodeBlockLanguageIsRenderedCorrectly()
    {
        var document = Doc(
            CodeBlock(
                new Dictionary<string, object?>
                {
                    ["language"] = "css",
                },
                Text("Example Text")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<pre><code class=\"language-css\">Example Text</code></pre>", result);
    }

    [Fact]
    public void CodeBlockLanguagePrefixIsConfigurable()
    {
        var document = Doc(
            CodeBlock(
                new Dictionary<string, object?>
                {
                    ["language"] = "css",
                },
                Text("Example Text")));

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
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<pre><code class=\"custom-language-prefix-css\">Example Text</code></pre>", result);
    }
}
