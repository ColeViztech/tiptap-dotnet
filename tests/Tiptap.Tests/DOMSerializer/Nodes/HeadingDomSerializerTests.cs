using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class HeadingDomSerializerTests
{
    [Fact]
    public void HeadingNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 2,
                },
                Text("Example Headline")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.EndsWith("<h2>Example Headline</h2>", result);
    }

    [Fact]
    public void ForbiddenHeadingLevelsAreTransformedToAllowedLevel()
    {
        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 7,
                },
                Text("Example Headline")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<h1>Example Headline</h1>", result);
    }

    [Fact]
    public void HeadingLevelsCanBeConfigured()
    {
        var editor = CreateEditorWithHeadingOptions(new Dictionary<string, object?>
        {
            ["levels"] = new List<int> { 1, 2, 3 },
        });

        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 3,
                },
                Text("Example Headline")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<h3>Example Headline</h3>", result);
    }

    [Fact]
    public void HeadingLevelsAreTransformedWhenNotAllowed()
    {
        var editor = CreateEditorWithHeadingOptions(new Dictionary<string, object?>
        {
            ["levels"] = new List<int> { 1, 2, 3 },
        });

        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 4,
                },
                Text("Example Headline")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<h1>Example Headline</h1>", result);
    }

    [Fact]
    public void ConfiguredHtmlAttributesAreRendered()
    {
        var editor = CreateEditorWithHeadingOptions(new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>
            {
                ["class"] = "custom-heading-class",
            },
        });

        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                },
                Text("Example Headline")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<h1 class=\"custom-heading-class\">Example Headline</h1>", result);
    }

    [Fact]
    public void CustomAttributesAreRenderedToo()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(new Dictionary<string, object?>
                {
                    ["heading"] = false,
                }),
                new CustomHeading(new Dictionary<string, object?>
                {
                    ["HTMLAttributes"] = new Dictionary<string, object?>
                    {
                        ["class"] = "custom-heading-class",
                    },
                }),
            }));

        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                    ["color"] = "red",
                },
                Text("Example Headline")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<h1 style=\"color: red\" class=\"custom-heading-class\">Example Headline</h1>", result);
    }

    [Fact]
    public void InlineStylesAreMergedProperly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(new Dictionary<string, object?>
                {
                    ["heading"] = false,
                }),
                new AnotherCustomHeading(new Dictionary<string, object?>
                {
                    ["HTMLAttributes"] = new Dictionary<string, object?>
                    {
                        ["style"] = "color: white; ",
                    },
                }),
            }));

        var document = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                    ["color"] = "red",
                },
                Text("Example Headline")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<h1 style=\"background-color: red; color: white\">Example Headline</h1>", result);
    }

    private static EditorClass CreateEditorWithHeadingOptions(IDictionary<string, object?> headingOptions)
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(new Dictionary<string, object?>
                {
                    ["heading"] = headingOptions,
                }),
            }));
    }

    private class CustomHeading : Heading
    {
        public CustomHeading(IDictionary<string, object?>? options = null)
            : base(options)
        {
        }

        public override IDictionary<string, AttributeConfiguration> AddAttributes()
        {
            var attributes = new Dictionary<string, AttributeConfiguration>(base.AddAttributes());

            attributes["color"] = new AttributeConfiguration
            {
                RenderHTML = attrs =>
                {
                    if (attrs is not IDictionary<string, object?> dictionary || !dictionary.TryGetValue("color", out var value) || value == null)
                    {
                        return null;
                    }

                    return new Dictionary<string, object?>
                    {
                        ["style"] = $"color: {value};",
                    };
                },
            };

            return attributes;
        }
    }

    private class AnotherCustomHeading : Heading
    {
        public AnotherCustomHeading(IDictionary<string, object?>? options = null)
            : base(options)
        {
        }

        public override IDictionary<string, AttributeConfiguration> AddAttributes()
        {
            var attributes = new Dictionary<string, AttributeConfiguration>(base.AddAttributes());

            attributes["color"] = new AttributeConfiguration
            {
                RenderHTML = attrs =>
                {
                    if (attrs is not IDictionary<string, object?> dictionary || !dictionary.TryGetValue("color", out var value) || value == null)
                    {
                        return null;
                    }

                    return new Dictionary<string, object?>
                    {
                        ["style"] = $"background-color: {value};",
                    };
                },
            };

            return attributes;
        }
    }
}
