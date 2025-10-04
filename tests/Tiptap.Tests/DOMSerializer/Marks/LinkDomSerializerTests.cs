using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Marks;

public class LinkDomSerializerTests
{
    [Fact]
    public void LinkMarkGetsRenderedCorrectly()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                    })));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://tiptap.dev\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">Example Link</a>", result);
    }

    [Fact]
    public void LinkMarkHasSupportForRel()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["rel"] = "noopener",
                    })));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://tiptap.dev\" rel=\"noopener\" target=\"_blank\">Example Link</a>", result);
    }

    [Fact]
    public void LinkMarkHasSupportForClass()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["rel"] = "noopener",
                        ["class"] = "tiptap",
                    })));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://tiptap.dev\" rel=\"noopener\" class=\"tiptap tiptap\" target=\"_blank\">Example Link</a>", result);
    }

    [Fact]
    public void LinkMarkHasSupportForTarget()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["target"] = "_self",
                    })));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://tiptap.dev\" target=\"_self\" rel=\"noopener noreferrer nofollow\">Example Link</a>", result);
    }

    [Fact]
    public void LinkWithMarksGeneratesCleanOutput()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example ",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://example.com",
                    })),
            Text(
                "Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://example.com",
                    }),
                Mark("bold")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://example.com\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">Example <strong>Link</strong></a>", result);
    }

    [Fact]
    public void LinkWithMarksInsideNodeGeneratesCleanOutput()
    {
        var editor = CreateEditor();

        var document = Doc(
            Paragraph(
                Text(
                    "Example ",
                    Mark(
                        "link",
                        new Dictionary<string, object?>
                        {
                            ["href"] = "https://example.com",
                        })),
                Text(
                    "Link",
                    Mark(
                        "link",
                        new Dictionary<string, object?>
                        {
                            ["href"] = "https://example.com",
                        }),
                    Mark("bold"))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><a href=\"https://example.com\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">Example <strong>Link</strong></a></p>", result);
    }

    [Fact]
    public void LinkMarkCanDisableRel()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["rel"] = null,
                    })));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://tiptap.dev\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">Example Link</a>", result);
    }

    [Fact]
    public void LinkMarkCanDisableTarget()
    {
        var editor = CreateEditor();

        var document = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["target"] = null,
                    })));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<a href=\"https://tiptap.dev\" target=\"_blank\" rel=\"noopener noreferrer nofollow\">Example Link</a>", result);
    }

    private static EditorClass CreateEditor()
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));
    }
}
