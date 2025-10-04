using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class HorizontalRuleDomParserTests
{
    [Fact]
    public void HorizontalRuleGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>Horizontal</p><hr /><p>Rule</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Horizontal")),
            HorizontalRule(),
            Paragraph(
                Text("Rule")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
