using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Models;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class ParseHtmlPriorityDomParserTests
{
    [Fact]
    public void PriorityForParsingHtml()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new HighPriorityParagraphNode(),
            }));

        var result = editor
            .SetContent("<p>Example</p>")
            .GetDocument();

        var expected = Doc(
            new ProseMirrorNode
            {
                Type = "highPriorityParagraph",
                Content = new List<ProseMirrorNode>
                {
                    Text("Example"),
                },
            });

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    private sealed class HighPriorityParagraphNode : Node
    {
        public override string Name => "highPriorityParagraph";

        public override IEnumerable<ParseRule> ParseHTML(object? context = null)
        {
            yield return new ParseRule
            {
                Tag = "p",
                Priority = 60,
            };
        }
    }
}
