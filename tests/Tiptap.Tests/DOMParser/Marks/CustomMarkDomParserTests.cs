using AngleSharp.Dom;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class CustomMarkDomParserTests
{
    [Fact]
    public void CustomMarkParsesAttributes()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new CustomMark(),
                }))
            .SetContent("<p><span data-foo=\"bar\" fruit=\"banana\">Example</span> text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Example",
                    Mark(
                        "custom",
                        new Dictionary<string, object?>
                        {
                            ["foo"] = "bar",
                            ["fruit"] = "banana",
                        })),
                Text(" text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    private sealed class CustomMark : Mark
    {
        public override string Name => "custom";

        public override IEnumerable<ParseRule> ParseHTML(object? context = null)
        {
            yield return new ParseRule
            {
                Tag = "span",
                GetAttrs = node =>
                {
                    if (node is not IElement element)
                    {
                        return null;
                    }

                    var attributes = new Dictionary<string, object?>();

                    var foo = element.GetAttribute("data-foo");
                    if (!string.IsNullOrEmpty(foo))
                    {
                        attributes["foo"] = foo;
                    }

                    var fruit = element.GetAttribute("fruit");
                    if (!string.IsNullOrEmpty(fruit))
                    {
                        attributes["fruit"] = fruit;
                    }

                    return attributes;
                },
            };
        }

        public override IDictionary<string, AttributeConfiguration> AddAttributes()
        {
            return new Dictionary<string, AttributeConfiguration>
            {
                ["foo"] = new AttributeConfiguration(),
                ["fruit"] = new AttributeConfiguration(),
            };
        }
    }
}
