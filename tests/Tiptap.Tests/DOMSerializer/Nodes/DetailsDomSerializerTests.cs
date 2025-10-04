using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class DetailsDomSerializerTests
{
    private static EditorClass CreateEditor(Details details)
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                details,
                new DetailsSummary(),
                new DetailsContent(),
            }));
    }

    [Fact]
    public void DetailsNodeGetsRenderedCorrectly()
    {
        var editor = CreateEditor(new Details());

        var document = Doc(
            Details(
                DetailsSummary(
                    Text("Summary Text")),
                DetailsContent(
                    Paragraph(
                        Text("Content Text")))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<details><summary>Summary Text</summary><div data-type=\"detailsContent\"><p>Content Text</p></div></details>", result);
    }

    [Fact]
    public void DetailsNodeWithOpenTrueRendersOpenAttributeWhenPersistIsEnabled()
    {
        var editor = CreateEditor(new Details(new Dictionary<string, object?>
        {
            ["persist"] = true,
        }));

        var document = Doc(
            Details(
                new Dictionary<string, object?>
                {
                    ["open"] = true,
                },
                DetailsSummary(
                    Text("Summary Text")),
                DetailsContent(
                    Paragraph(
                        Text("Content Text")))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<details open=\"open\" class=\"is-open\"><summary>Summary Text</summary><div data-type=\"detailsContent\"><p>Content Text</p></div></details>", result);
    }

    [Fact]
    public void DetailsNodeWithOpenFalseDoesNotRenderOpenAttributeWhenPersistIsEnabled()
    {
        var editor = CreateEditor(new Details(new Dictionary<string, object?>
        {
            ["persist"] = true,
        }));

        var document = Doc(
            Details(
                new Dictionary<string, object?>
                {
                    ["open"] = false,
                },
                DetailsSummary(
                    Text("Summary Text")),
                DetailsContent(
                    Paragraph(
                        Text("Content Text")))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<details><summary>Summary Text</summary><div data-type=\"detailsContent\"><p>Content Text</p></div></details>", result);
    }
}
