using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class ImageDomParserTests
{
    private static EditorClass CreateEditor()
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Image(),
            }));
    }

    [Fact]
    public void ImageGetsRenderedCorrectly()
    {
        var editor = CreateEditor();

        var result = editor
            .SetContent("<img src=\"https://example.com/eggs.png\" alt=\"The Finished Dish\" title=\"Eggs in a dish\" />")
            .GetDocument();

        var expected = Doc(
            Image(new Dictionary<string, object?>
            {
                ["src"] = "https://example.com/eggs.png",
                ["alt"] = "The Finished Dish",
                ["title"] = "Eggs in a dish",
            }));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ImageGetsRenderedCorrectlyWhenTitleIsMissing()
    {
        var editor = CreateEditor();

        var result = editor
            .SetContent("<img src=\"https://example.com/eggs.png\" alt=\"The Finished Dish\" />")
            .GetDocument();

        var expected = Doc(
            Image(new Dictionary<string, object?>
            {
                ["src"] = "https://example.com/eggs.png",
                ["alt"] = "The Finished Dish",
            }));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ImageGetsRenderedCorrectlyWhenAltIsMissing()
    {
        var editor = CreateEditor();

        var result = editor
            .SetContent("<img src=\"https://example.com/eggs.png\" title=\"Eggs in a dish\" />")
            .GetDocument();

        var expected = Doc(
            Image(new Dictionary<string, object?>
            {
                ["src"] = "https://example.com/eggs.png",
                ["title"] = "Eggs in a dish",
            }));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
