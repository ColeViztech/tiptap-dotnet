using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class CodeBlockHighlightDomSerializerTests
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
    public void CodeBlockHighlightAddsSyntaxHighlightingToCodeBlocks()
    {
        var editor = CreateEditor(new CodeBlockHighlight());

        var result = editor
            .SetContent("<pre><code>body { display: none }</code></pre>")
            .GetHTML();

        Assert.Equal("<pre><code>body { display: none }</code></pre><code>body { display: none }</code>", result);
    }

    [Fact]
    public void CodeBlockHighlightUsesTheSpecifiedLanguage()
    {
        var editor = CreateEditor(new CodeBlockHighlight());

        var result = editor
            .SetContent("<pre><code class=\"hljs php\">&lt;?php phpinfo()</code></pre>")
            .GetHTML();

        Assert.Equal("<pre><code class=\"hljs php\">&lt;?php phpinfo()</code></pre><code><?php phpinfo()</code>", result);
    }

    [Fact]
    public void CodeBlockHighlightUsesTheConfiguredLanguageClassPrefix()
    {
        var editor = CreateEditor(new CodeBlockHighlight(new Dictionary<string, object?>
        {
            ["languageClassPrefix"] = "foo ",
        }));

        var result = editor
            .SetContent("<pre><code class=\"foo php\">&lt;?php phpinfo()</code></pre>")
            .GetHTML();

        Assert.Equal("<pre><code class=\"foo php\">&lt;?php phpinfo()</code></pre><code><?php phpinfo()</code>", result);
    }

    [Fact]
    public void CodeBlockHighlightFallsBackToPlainPreAndCodeTags()
    {
        var editor = CreateEditor(new CodeBlockHighlight());

        var result = editor
            .SetContent("<pre><code class=\"WRONG PREFIX php\">&lt;?php phpinfo()</code></pre>")
            .GetHTML();

        Assert.Equal("<pre><code>&lt;?php phpinfo()</code></pre><code><?php phpinfo()</code>", result);
    }
}
