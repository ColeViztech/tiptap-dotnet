using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class LinkDomParserTests
{
    private static readonly string[] ValidUrls =
    {
        "https://example.com",
        "http://example.com",
        "/same-site/index.html",
        "../relative.html",
        "mailto:info@example.com",
        "ftp://info@example.com",
    };

    private static readonly string[] InvalidUrls =
    {
        "javascript:alert(window.origin)",
        "jAvAsCrIpT:alert(window.origin)",
        "\x00javascript:alert(window.origin)",
        "\x01javascript:alert(window.origin)",
        "\x02javascript:alert(window.origin)",
        "\x03javascript:alert(window.origin)",
        "\x04javascript:alert(window.origin)",
        "\x05javascript:alert(window.origin)",
        "\x06javascript:alert(window.origin)",
        "\x07javascript:alert(window.origin)",
        "\x08javascript:alert(window.origin)",
        "\x09javascript:alert(window.origin)",
        "\x0ajavascript:alert(window.origin)",
        "\x0bjavascript:alert(window.origin)",
        "\x0cjavascript:alert(window.origin)",
        "\x0djavascript:alert(window.origin)",
        "\x0ejavascript:alert(window.origin)",
        "\x0fjavascript:alert(window.origin)",
        "\x10javascript:alert(window.origin)",
        "\x11javascript:alert(window.origin)",
        "\x12javascript:alert(window.origin)",
        "\x13javascript:alert(window.origin)",
        "\x14javascript:alert(window.origin)",
        "\x15javascript:alert(window.origin)",
        "\x16javascript:alert(window.origin)",
        "\x17javascript:alert(window.origin)",
        "\x18javascript:alert(window.origin)",
        "\x19javascript:alert(window.origin)",
        "\x1ajavascript:alert(window.origin)",
        "\x1bjavascript:alert(window.origin)",
        "\x1cjavascript:alert(window.origin)",
        "\x1djavascript:alert(window.origin)",
        "\x1ejavascript:alert(window.origin)",
        "\x1fjavascript:alert(window.origin)",
        "java\x09script:alert(window.origin)",
        "java\x0ascript:alert(window.origin)",
        "java\x0dscript:alert(window.origin)",
        "javascript\x09:alert(window.origin)",
        "javascript\x0a:alert(window.origin)",
        "javascript\x0d:alert(window.origin)",
    };

    [Fact]
    public void LinkTagGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Link(),
                }));

        var result = editor
            .SetContent("<a href=\"https://tiptap.dev\">Example Link</a>")
            .GetDocument();

        var expected = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                    })));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void LinkSupportsRelAttribute()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Link(),
                }))
            .SetContent("<a href=\"https://tiptap.dev\" rel=\"noopener\">Example Link</a>")
            .GetDocument();

        var expected = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["rel"] = "noopener",
                    })));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void LinkSupportsClassAttribute()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Link(),
                }))
            .SetContent("<a class=\"tiptap\" href=\"https://tiptap.dev\">Example Link</a>")
            .GetDocument();

        var expected = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["class"] = "tiptap",
                    })));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void LinkSupportsTargetAttribute()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Link(),
                }))
            .SetContent("<a href=\"https://tiptap.dev\" target=\"_blank\">Example Link</a>")
            .GetDocument();

        var expected = Doc(
            Text(
                "Example Link",
                Mark(
                    "link",
                    new Dictionary<string, object?>
                    {
                        ["href"] = "https://tiptap.dev",
                        ["target"] = "_blank",
                    })));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Theory]
    [MemberData(nameof(GetValidUrls))]
    public void LinkMarkOutputsHrefForValidJsonSchemas(string url)
    {
        var document = Doc(
            Paragraph(
                Text(
                    "Click me",
                    Mark(
                        "link",
                        new Dictionary<string, object?>
                        {
                            ["href"] = url,
                        }))));

        var editor = new EditorClass(new EditorOptions(
            Content: document,
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var result = editor.GetHTML();

        Assert.Contains(url, result);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUrls))]
    public void LinkMarkDoesNotOutputHrefForInvalidJsonSchemas(string url)
    {
        var document = Doc(
            Paragraph(
                Text(
                    "Click me",
                    Mark(
                        "link",
                        new Dictionary<string, object?>
                        {
                            ["href"] = url,
                        }))));

        var editor = new EditorClass(new EditorOptions(
            Content: document,
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var result = editor.GetHTML();

        Assert.DoesNotContain(url, result);
    }

    [Theory]
    [MemberData(nameof(GetValidUrls))]
    public void LinkMarkOutputsHrefForValidHtmlSchemas(string url)
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var result = editor
            .SetContent($"<p><a href=\"{url}\">Click me</a></p>")
            .GetHTML();

        Assert.Contains(url, result);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUrls))]
    public void LinkMarkDoesNotOutputHrefForInvalidHtmlSchemas(string url)
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Link(),
            }));

        var result = editor
            .SetContent($"<p><a href=\"{url}\">Click me</a></p>")
            .GetJSON();

        Assert.DoesNotContain(url, result);
    }

    public static IEnumerable<object[]> GetValidUrls()
    {
        foreach (var url in ValidUrls)
        {
            yield return new object[] { url };
        }
    }

    public static IEnumerable<object[]> GetInvalidUrls()
    {
        foreach (var url in InvalidUrls)
        {
            yield return new object[] { url };
        }
    }
}
