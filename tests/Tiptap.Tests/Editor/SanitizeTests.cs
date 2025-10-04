using Tiptap.Core.Models;
using Tiptap.Tests;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class SanitizeTests
{
    [Fact]
    public void UnknownNodesAreRemovedFromDocument()
    {
        var document = CreateFooDocument();

        var result = new EditorClass()
            .SetContent(document)
            .GetDocument();

        Assert.Equal(JsonTestHelper.Serialize(document), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void UnknownNodesAreRemovedFromDocumentUsingSanitize()
    {
        var document = CreateFooDocument();

        var result = new EditorClass()
            .Sanitize(document) as ProseMirrorDocument;

        Assert.NotNull(result);
        Assert.Equal(JsonTestHelper.Serialize(document), JsonTestHelper.Serialize(result!));
    }

    [Fact]
    public void UnknownHtmlTagsAreRemoved()
    {
        var html = new EditorClass()
            .SetContent("<p>Example Text<script>alert(\"HACKED\");</script></p>")
            .GetHTML();

        Assert.Equal("<p>Example Text</p>", html);
    }

    [Fact]
    public void UnknownHtmlTagsAreRemovedUsingSanitize()
    {
        var html = new EditorClass()
            .Sanitize("<p>Example Text<script>alert(\"HACKED\");</script></p>") as string;

        Assert.Equal("<p>Example Text</p>", html);
    }

    [Fact]
    public void UnknownNodesAreRemovedFromJson()
    {
        var document = CreateFooDocument();
        var json = JsonTestHelper.Serialize(document);

        var result = new EditorClass()
            .SetContent(json)
            .GetJSON();

        Assert.Equal(json, result);
    }

    [Fact]
    public void UnknownNodesAreRemovedFromJsonUsingSanitize()
    {
        var document = CreateFooDocument();
        var json = JsonTestHelper.Serialize(document);

        var result = new EditorClass()
            .Sanitize(json) as string;

        Assert.Equal(json, result);
    }

    private static ProseMirrorDocument CreateFooDocument()
    {
        return new ProseMirrorDocument
        {
            Type = "doc",
            Content =
            [
                new ProseMirrorNode
                {
                    Type = "foo",
                    Content =
                    [
                        new ProseMirrorNode
                        {
                            Type = "foo",
                            Text = "Example Text",
                        },
                    ],
                },
            ],
        };
    }
}
