using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class MentionDomParserTests
{
    [Fact]
    public void UserMentionGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Mention(),
            }));

        var result = editor
            .SetContent("<p>Hey <span data-type=\"mention\" data-id=\"123\"></span>, was geht?</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Hey "),
                Mention(new Dictionary<string, object?>
                {
                    ["id"] = "123",
                }),
                Text(", was geht?")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
