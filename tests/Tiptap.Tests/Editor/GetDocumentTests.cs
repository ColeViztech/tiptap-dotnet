using Tiptap.Core.Models;
using Tiptap.Tests;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class GetDocumentTests
{
    [Fact]
    public void GetDocumentReturnsDocument()
    {
        var result = new EditorClass()
            .SetContent("<p>Example Text</p>")
            .GetDocument();

        var expected = new ProseMirrorDocument
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

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
