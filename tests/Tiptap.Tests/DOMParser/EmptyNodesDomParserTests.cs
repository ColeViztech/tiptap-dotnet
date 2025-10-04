using System.Collections.Generic;
using Tiptap.Core.Models;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class EmptyNodesDomParserTests
{
    [Fact]
    public void ParsingMustNotFailOnEmptyNodes()
    {
        var html = "<p><img /></p><p><img /></p>";

        var result = new EditorClass()
            .SetContent(html)
            .GetDocument();

        var expected = Doc(
            new ProseMirrorNode
            {
                Type = "paragraph",
                Content = new List<ProseMirrorNode>(),
            },
            new ProseMirrorNode
            {
                Type = "paragraph",
                Content = new List<ProseMirrorNode>(),
            });

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
