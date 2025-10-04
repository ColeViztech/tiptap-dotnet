using EditorClass = Tiptap.Core.Editor;
using Tiptap.Core.Models;

namespace Tiptap.Tests.Editor;

public class GetHtmlTests
{
    [Fact]
    public void GetHtmlReturnsHtml()
    {
        var document = new ProseMirrorDocument
        {
            Type = "doc",
            Content =
            [
                new ProseMirrorNode
                {
                    Type = "paragraph",
                    Content =
                    [
                        new ProseMirrorNode
                        {
                            Type = "text",
                            Text = "Example Text",
                        },
                    ],
                },
            ],
        };

        var html = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>Example Text</p>", html);
    }
}
