using System.Collections.Generic;
using System.Linq;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class DetailsDomParserTests
{
    private static EditorClass CreateEditor(IEnumerable<Extension> extensions)
    {
        return new EditorClass(new EditorOptions(
            Extensions: extensions.ToArray()));
    }

    [Fact]
    public void DetailsGetsRenderedCorrectly()
    {
        var editor = CreateEditor(new Extension[]
        {
            new StarterKit(),
            new Details(),
            new DetailsSummary(),
            new DetailsContent(),
        });

        var result = editor
            .SetContent("<details><summary>Summary</summary><div data-type=\"detailsContent\"><p>Content</p></div></details>")
            .GetDocument();

        var expected = Doc(
            Details(
                DetailsSummary(
                    Text("Summary")),
                DetailsContent(new Dictionary<string, object?>(),
                    Paragraph(
                        Text("Content")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void DetailsOpenAttributeIsIgnoredByDefault()
    {
        var editor = CreateEditor(new Extension[]
        {
            new StarterKit(),
            new Details(),
            new DetailsSummary(),
            new DetailsContent(),
        });

        var result = editor
            .SetContent("<details open><summary>Summary</summary><div data-type=\"detailsContent\"><p>Content</p></div></details>")
            .GetDocument();

        var expected = Doc(
            Details(
                DetailsSummary(
                    Text("Summary")),
                DetailsContent(new Dictionary<string, object?>(),
                    Paragraph(
                        Text("Content")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void DetailsOpenAttributeIsParsedWhenPersistIsEnabled()
    {
        var editor = CreateEditor(new Extension[]
        {
            new StarterKit(),
            new Details(new Dictionary<string, object?>
            {
                ["persist"] = true,
            }),
            new DetailsSummary(),
            new DetailsContent(),
        });

        var result = editor
            .SetContent("<details open><summary>Summary</summary><div data-type=\"detailsContent\"><p>Content</p></div></details>")
            .GetDocument();

        var expected = Doc(
            Details(
                new Dictionary<string, object?>
                {
                    ["open"] = true,
                },
                DetailsSummary(
                    Text("Summary")),
                DetailsContent(new Dictionary<string, object?>(),
                    Paragraph(
                        Text("Content")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void DetailsWithoutOpenAttributeSetsOpenToFalseWhenPersistIsEnabled()
    {
        var editor = CreateEditor(new Extension[]
        {
            new StarterKit(),
            new Details(new Dictionary<string, object?>
            {
                ["persist"] = true,
            }),
            new DetailsSummary(),
            new DetailsContent(),
        });

        var result = editor
            .SetContent("<details><summary>Summary</summary><div data-type=\"detailsContent\"><p>Content</p></div></details>")
            .GetDocument();

        var expected = Doc(
            Details(
                new Dictionary<string, object?>
                {
                    ["open"] = false,
                },
                DetailsSummary(
                    Text("Summary")),
                DetailsContent(new Dictionary<string, object?>(),
                    Paragraph(
                        Text("Content")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
