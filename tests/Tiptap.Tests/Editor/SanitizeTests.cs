using System;
using System.Text.Json;
using AngleSharp.Html.Parser;
using Tiptap.Core.Models;
using Tiptap.Tests;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class SanitizeTests
{
    [Fact]
    public void UnknownNodesAreRemovedFromDocument()
    {
        var document = CreateFooDocument();

        var result = new EditorClass()
            .SetContent(document)
            .GetDocument();

        Assert.Equal(JsonTestHelper.Serialize(document), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void UnknownNodesAreRemovedFromDocumentUsingSanitize()
    {
        var result = new EditorClass()
            .Sanitize(CreateFooDocument()) as ProseMirrorDocument;

        Assert.NotNull(result);
        Assert.Equal("doc", result!.Type);
        Assert.NotNull(result.Content);
        Assert.Single(result.Content!);
        var textNode = result.Content![0];
        Assert.Equal("text", textNode.Type);
        Assert.Equal("Example Text", textNode.Text);
    }

    [Fact]
    public void UnknownHtmlTagsAreRemoved()
    {
        var html = new EditorClass()
            .SetContent("<p>Example Text<script>alert(\"HACKED\");</script></p>")
            .GetHTML();

        Assert.Equal("<p>Example Text</p>", html);
    }

    [Fact]
    public void UnknownHtmlTagsAreRemovedUsingSanitize()
    {
        var html = new EditorClass()
            .Sanitize("<p>Example Text<script>alert(\"HACKED\");</script></p>") as string;

        Assert.Equal("<p>Example Text</p>", html);
    }

    [Fact]
    public void MathMlIntegrationPointPayloadIsSafeAfterReparse()
    {
        const string payload = "<math><annotation-xml encoding=\"text/html\"><title><a encoding=\"</title><img src=x onerror=alert()>\"></annotation-xml></math>";
        var parser = new HtmlParser();

        var parsed = parser.ParseDocument(payload);
        Assert.NotNull(parsed.QuerySelector("img"));

        var sanitized = new EditorClass()
            .Sanitize(payload) as string;

        Assert.NotNull(sanitized);
        Assert.DoesNotContain("onerror", sanitized!, StringComparison.OrdinalIgnoreCase);

        var reparsed = parser.ParseDocument(sanitized);
        Assert.Null(reparsed.QuerySelector("img[onerror]"));
    }

    [Fact]
    public void UnknownNodesAreRemovedFromJson()
    {
        var document = CreateFooDocument();
        var json = JsonTestHelper.Serialize(document);

        var result = new EditorClass()
            .SetContent(json)
            .GetJSON();

        Assert.Equal(json, result);
    }

    [Fact]
    public void UnknownNodesAreRemovedFromJsonUsingSanitize()
    {
        var json = JsonTestHelper.Serialize(CreateFooDocument());

        var sanitized = new EditorClass()
            .Sanitize(json) as string;

        Assert.NotNull(sanitized);
        Assert.DoesNotContain("foo", sanitized!, StringComparison.OrdinalIgnoreCase);

        var parsed = JsonSerializer.Deserialize<ProseMirrorDocument>(sanitized!);
        Assert.NotNull(parsed);
        Assert.NotNull(parsed!.Content);
        Assert.Single(parsed.Content!);
        Assert.Equal("text", parsed.Content![0].Type);
        Assert.Equal("Example Text", parsed.Content![0].Text);
    }

    [Fact]
    public void JsonInputWithJavascriptHrefIsSanitized()
    {
        var maliciousJson = """
        {
            "type": "doc",
            "content": [
                {
                    "type": "paragraph",
                    "content": [
                        {
                            "type": "text",
                            "text": "Click me",
                            "marks": [
                                {
                                    "type": "link",
                                    "attrs": { "href": "javascript:alert(1)" }
                                }
                            ]
                        }
                    ]
                }
            ]
        }
        """;

        var sanitized = new EditorClass()
            .Sanitize(maliciousJson) as string;

        Assert.NotNull(sanitized);
        Assert.DoesNotContain("javascript:", sanitized!, StringComparison.OrdinalIgnoreCase);

        var parsed = JsonSerializer.Deserialize<ProseMirrorDocument>(sanitized!);
        Assert.NotNull(parsed);
        var textNode = parsed!.Content?[0].Content?[0];
        Assert.NotNull(textNode);
        Assert.True(textNode!.Marks == null || textNode.Marks.Count == 0);
    }

    [Fact]
    public void DocumentInputWithJavascriptHrefIsSanitized()
    {
        var document = new ProseMirrorDocument
        {
            Type = "doc",
            Content =
            [
                new ProseMirrorNode
                {
                    Type = "paragraph",
                    Content =
                    [
                        new ProseMirrorNode
                        {
                            Type = "text",
                            Text = "Click me",
                            Marks =
                            [
                                new ProseMirrorMark
                                {
                                    Type = "link",
                                    Attrs = new Dictionary<string, object?>
                                    {
                                        ["href"] = "javascript:alert(1)",
                                    },
                                },
                            ],
                        },
                    ],
                },
            ],
        };

        var sanitized = new EditorClass()
            .Sanitize(document) as ProseMirrorDocument;

        Assert.NotNull(sanitized);
        var textNode = sanitized!.Content?[0].Content?[0];
        Assert.NotNull(textNode);
        Assert.True(textNode!.Marks == null || textNode.Marks.Count == 0);
    }

    private static ProseMirrorDocument CreateFooDocument()
    {
        return new ProseMirrorDocument
        {
            Type = "doc",
            Content =
            [
                new ProseMirrorNode
                {
                    Type = "foo",
                    Content =
                    [
                        new ProseMirrorNode
                        {
                            Type = "foo",
                            Text = "Example Text",
                        },
                    ],
                },
            ],
        };
    }
}
