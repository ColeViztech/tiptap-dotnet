using Tiptap.Core;
using Tiptap.Core.Models;
using Tiptap.Tests;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class SetContentTests
{
    [Fact]
    public void JsonStringsAreDetected()
    {
        var json = "{\n        \"type\": \"doc\",\n        \"content\": [\n            {\n                \"type\": \"paragraph\",\n                \"content\": [\n                    {\n                        \"type\": \"text\",\n                        \"text\": \"Example Text\"\n                    }\n                ]\n            }\n        ]\n    }";

        var document = new EditorClass()
            .SetContent(json)
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

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(document));
    }

    [Fact]
    public void DocumentsAreDetected()
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

        var result = new EditorClass()
            .SetContent(document)
            .GetDocument();

        Assert.Equal(JsonTestHelper.Serialize(document), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void HtmlIsDetected()
    {
        var document = new EditorClass()
            .SetContent("<p>Example <strong>Text</strong></p>")
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
                            Text = "Example ",
                        },
                        new ProseMirrorNode
                        {
                            Type = "text",
                            Text = "Text",
                            Marks =
                            [
                                new ProseMirrorMark
                                {
                                    Type = "bold",
                                },
                            ],
                        },
                    ],
                },
            ],
        };

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(document));
    }

    [Fact]
    public void ContentCanBeProvidedViaOptions()
    {
        var document = new EditorClass(new EditorOptions(Content: "<p>Example Text</p>"))
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

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(document));
    }
}
