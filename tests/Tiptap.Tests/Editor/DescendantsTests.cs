using Tiptap.Core.Models;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class DescendantsTests
{
    [Fact]
    public void DescendantsLoopsThroughAllNodesRecursively()
    {
        var document = new ProseMirrorDocument
        {
            Type = "doc",
            Content =
            [
                new ProseMirrorNode
                {
                    Type = "bulletList",
                    Content =
                    [
                        new ProseMirrorNode
                        {
                            Type = "listItem",
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
                                            Text = "Example",
                                        },
                                    ],
                                },
                            ],
                        },
                        new ProseMirrorNode
                        {
                            Type = "listItem",
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
                                            Text = "Text ",
                                        },
                                        new ProseMirrorNode
                                        {
                                            Type = "text",
                                            Text = "Test",
                                            Marks =
                                            [
                                                new ProseMirrorMark
                                                {
                                                    Type = "italic",
                                                },
                                            ],
                                        },
                                    ],
                                },
                            ],
                        },
                    ],
                },
                new ProseMirrorNode
                {
                    Type = "paragraph",
                    Content =
                    [
                        new ProseMirrorNode
                        {
                            Type = "text",
                            Text = "Example",
                        },
                    ],
                },
            ],
        };

        var editor = new EditorClass().SetContent(document);
        var result = new List<string?>();

        editor.Descendants(node => result.Add(node.Type));

        Assert.Equal(new[]
        {
            "doc",
            "bulletList",
            "listItem",
            "paragraph",
            "listItem",
            "paragraph",
            "paragraph",
        }, result);
    }

    [Fact]
    public void DescendantsAllowsUpdatingNodeAttributes()
    {
        var document = new ProseMirrorDocument
        {
            Type = "doc",
            Content =
            [
                new ProseMirrorNode
                {
                    Type = "heading",
                    Attrs = new Dictionary<string, object?>
                    {
                        ["level"] = 2,
                    },
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
            .Descendants(node =>
            {
                if (!string.Equals(node.Type, "heading", StringComparison.Ordinal))
                {
                    return;
                }

                node.Attrs!["level"] = 1;
            })
            .GetHTML();

        Assert.Equal("<h1>Example Text</h1>", html);
    }
}
