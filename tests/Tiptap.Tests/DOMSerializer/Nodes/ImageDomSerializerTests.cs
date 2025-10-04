using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class ImageDomSerializerTests
{
    [Fact]
    public void ImageNodeGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Image(),
            }));

        var document = Doc(
            Image(
                new Dictionary<string, object?>
                {
                    ["alt"] = "an image",
                    ["src"] = "image/source",
                    ["title"] = "The image title",
                }));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<img src=\"image/source\" alt=\"an image\" title=\"The image title\">", result);
    }
}
