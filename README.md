# Tiptap for .NET

[![NuGet](https://img.shields.io/nuget/v/Tiptap.NET.svg)](https://www.nuget.org/packages/Tiptap.NET)
[![Build](https://github.com/ColeViztech/tiptap-dotnet/actions/workflows/build.yml/badge.svg)](https://github.com/ColeViztech/tiptap-dotnet/actions)

This repository hosts a .NET Conversion of the [tiptap-php](https://github.com/ueberdosis/tiptap-php), It would not have been possible without them. It targets .NET 8+ and mirrors the API surface of the original PHP package so that server-side code can parse, sanitize, and transform Tiptap content without relying on Node.js. The implementation now ships with an automated test suite (`dotnet test`) that covers the HTML, JSON, and text serializers as well as the extension options that tend to drift between ecosystems.

## Repository layout

```
Tiptap.sln               # Solution that includes the core library project
src/Tiptap.Core/         # C# source for the Tiptap.Core class library
```

## Installation

Once published:
```bash
dotnet add package Tiptap.NET
```
Secondary method:
The library is distributed as the `Tiptap.Core` class library that you can reference from any .NET solution:

```bash
# Clone or add the git submodule
git clone https://github.com/ColeViztech/tiptap-dotnet.git

# Restore dependencies and build (requires the .NET 8 SDK)
cd tiptap-dotnet
dotnet restore
dotnet build src/Tiptap.Core/Tiptap.Core.csproj
```

Add the project to your own solution with a project reference:

```bash
dotnet new console -n TiptapExample
cd TiptapExample
dotnet add reference ../tiptap-dotnet/src/Tiptap.Core/Tiptap.Core.csproj
```

## Usage

The entry point for all conversions is the `Editor` class. Instantiate it, configure the extensions you need, set the content, and then render the output format you want.

```csharp
using System.Text.Json;
using Tiptap.Core;
using Tiptap.Core.Extensions;

var editor = new Editor(new EditorOptions
{
    Extensions = new []
    {
        new StarterKit(),
    },
});

editor.SetContent("<p>Example Text</p>");

// Extract the ProseMirror document model
var document = editor.GetDocument();
Console.WriteLine(JsonSerializer.Serialize(document));
```

Every serializer (`GetJSON`, `GetHTML`, `GetText`) clones the internal document, so you can safely reuse the editor instance across requests.

### Convert Tiptap HTML to JSON

```csharp
using Tiptap.Core;

var editor = new Editor();
var json = editor
    .SetContent("<p>Example Text</p>")
    .GetJSON();

// json == "{\"type\":\"doc\",\"content\":[{...}]}"
```

You can deserialize the JSON into a strongly-typed `ProseMirrorDocument` when needed:

```csharp
using System.Text.Json;
using Tiptap.Core.Models;

var document = JsonSerializer.Deserialize<ProseMirrorDocument>(json);
```

### Convert Tiptap JSON to HTML

```csharp
using System.Text.Json;
using Tiptap.Core;

var json = "{\"type\":\"doc\",\"content\":[{\"type\":\"paragraph\",\"content\":[{\"type\":\"text\",\"text\":\"Example Text\"}]}]}";

var html = new Editor()
    .SetContent(json)
    .GetHTML();

// html == "<p>Example Text</p>"
```

Passing an anonymous object, record, or `ProseMirrorDocument` instance is also supported:

```csharp
var html = new Editor()
    .SetContent(new
    {
        type = "doc",
        content = new object[]
        {
            new
            {
                type = "paragraph",
                content = new object[]
                {
                    new { type = "text", text = "Example Text" },
                },
            },
        },
    })
    .GetHTML();
```

### Syntax highlighting for code blocks

Replace the default `CodeBlock` extension with `CodeBlockHighlight` to emit the CSS hooks expected by [highlight.js](https://highlightjs.org/) or any .NET-friendly syntax highlighting pipeline. After rendering you can hand the HTML to your preferred highlighter (for example, call into a .NET library such as [ColorCode-Universal](https://github.com/CommunityToolkit/ColorCode-Universal) or run highlight.js in the browser).

```csharp
using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;

var html = new Editor(new EditorOptions
{
    Extensions = new Extension[]
    {
        new StarterKit(new Dictionary<string, object?>
        {
            ["codeBlock"] = false,
        }),
        new CodeBlockHighlight(),
    },
})
.SetContent("<pre><code class=\"language-csharp\">Console.WriteLine(\\\"Hello\\\");</code></pre>")
.GetHTML();

// <pre><code class="hljs csharp">Console.WriteLine(&quot;Hello&quot;);</code></pre>
```

> Highlighting still requires a stylesheet. Load any highlight.js theme in your frontend: `<link rel="stylesheet" href="//unpkg.com/@highlightjs/cdn-assets@11.8.0/styles/default.min.css">`.

### Convert content to plain text

Use `GetText` to strip markup and retrieve plain text for search indexes or previews. The block separator defaults to two line breaks and can be overridden:

```csharp
using System.Collections.Generic;
using Tiptap.Core;

var text = new Editor()
    .SetContent("<h1>Heading</h1><p>Paragraph</p>")
    .GetText(new Dictionary<string, object?>
    {
        ["blockSeparator"] = "\n",
    });

// text == "Heading\nParagraph"
```

### Sanitize content

`Sanitize` parses the value with the configured schema and returns it in the same format. Invalid nodes and marks are stripped as part of schema application:

```csharp
using Tiptap.Core;

var editor = new Editor();

var sanitized = editor.Sanitize("<p onclick=\"alert(1)\">Hello</p>");
// sanitized == "<p>Hello</p>"

var sanitizedJson = editor.Sanitize("{\"type\":\"doc\",\"content\":[]}");
// sanitizedJson == "{\"type\":\"doc\",\"content\":[]}" (unchanged JSON)
```

### Modifying the content

Walk the document tree with `Descendants` to add or change attributes. The editor automatically re-applies the schema afterwards:

```csharp
using System.Collections.Generic;
using Tiptap.Core;

var editor = new Editor();

editor
    .SetContent("<p>plain paragraph</p>")
    .Descendants(node =>
    {
        if (node.Type == "paragraph")
        {
            node.Attrs ??= new Dictionary<string, object?>();
            node.Attrs["data-level"] = "intro";
        }
    });

var html = editor.GetHTML();
// <p data-level="intro">plain paragraph</p>
```

## Configuration

Customize the editor through `EditorOptions`:

```csharp
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;

var editor = new Editor(new EditorOptions
{
    Content = "<p>Initial Content</p>",
    Extensions = new Extension[]
    {
        new StarterKit(),
        new Highlight(),
    },
});
```

Passing `Content` initializes the document immediately. `Extensions` lets you swap or add nodes and marks.

## Extensions

The Starter Kit wires together the core Tiptap nodes and marks. Options are forwarded to the individual extensions, so you can enable/disable pieces or tweak their defaults.

### Configure extensions

Each extension accepts an optional dictionary of options. For instance, restrict headings to `h1` and `h2` and add custom HTML attributes:

```csharp
using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;

var editor = new Editor(new EditorOptions
{
    Extensions = new Extension[]
    {
        new StarterKit(new Dictionary<string, object?>
        {
            ["heading"] = new Dictionary<string, object?>
            {
                ["levels"] = new[] { 1, 2 },
                ["HTMLAttributes"] = new Dictionary<string, object?>
                {
                    ["class"] = "title",
                },
            },
        }),
    },
});
```

Disable pieces of the starter kit by setting them to `false` and provide an alternative:

```csharp
using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;

var editor = new Editor(new EditorOptions
{
    Extensions = new Extension[]
    {
        new StarterKit(new Dictionary<string, object?>
        {
            ["codeBlock"] = false,
        }),
        new CodeBlockHighlight(),
    },
});
```

### Extend existing extensions

Create your own extension by inheriting from `Node`, `Mark`, or `Extension`. Override the hooks you need:

```csharp
using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Models;

public class Notice : Node
{
    public Notice(IDictionary<string, object?>? options = null) : base(options) { }

    public override string Name => "notice";

    public override object? RenderHTML(ProseMirrorNode node) => new object?[]
    {
        "div",
        new Dictionary<string, object?>
        {
            ["class"] = "notice notice--" + (node.Attrs?["variant"] ?? "info"),
        },
        0,
    };
}

var editor = new Editor(new EditorOptions
{
    Extensions = new Extension[] { new StarterKit(), new Notice() },
});
```

Register global attributes or nested extensions by overriding `AddGlobalAttributes` or `AddExtensions` on your custom class—just like you would in the PHP or JavaScript ecosystems.

## Library behaviour differences between PHP and C#

The C# port deliberately keeps parity with the PHP release while surfacing the differences that naturally emerge from each runtime:

* **Error handling for invalid JSON.** The PHP library silently strips invalid array entries when you pass malformed JSON. The .NET port calls `JsonDocument.Parse` and `System.Text.Json.JsonSerializer.Deserialize`, so malformed payloads raise `JsonException`. The behaviour is covered by unit tests to prevent regressions and encourages early detection of data issues. 【F:src/Tiptap.Core/Editor.cs†L72-L118】【F:tests/Tiptap.Tests/DOMSerializer/WrongFormatDomSerializerTests.cs†L17-L78】
* **Attribute defaults and normalisation.** Node defaults follow the same shape as PHP, but the port validates attributes before rendering. For example, `Heading` still defaults to levels 1–6 and coerces invalid levels back into the allowed set, just like PHP, with tests ensuring the canonical level is preserved. 【F:src/Tiptap.Core/Nodes/Heading.cs†L17-L71】【F:tests/Tiptap.Tests/DOMSerializer/Nodes/HeadingDomSerializerTests.cs†L14-L209】
* **HTML encoding.** The serializer returns HTML that is safe by default. When you render code blocks with `CodeBlockHighlight` the .NET runtime uses `WebUtility.HtmlEncode`, so output contains entities such as `&quot;`. Normalise your downstream expectations accordingly—the PHP version emits double quotes directly. 【F:src/Tiptap.Core/Nodes/CodeBlockHighlight.cs†L14-L42】
* **Sanitisation strategy.** Both ports apply the schema to incoming content, but the .NET implementation always returns the content in the format you provided (`HTML`, `JSON`, or `ProseMirrorDocument`) to avoid lossy conversions. This keeps behaviour predictable when integrating with ASP.NET backends. 【F:src/Tiptap.Core/Editor.cs†L120-L151】

If you encounter additional discrepancies, please open an issue so we can either close the gap or document the reasoning behind it.

## Testing

The repository includes a comprehensive xUnit test suite that mirrors the PHP fixtures. Run it locally with:

```bash
dotnet test
```

The suite covers DOM parsing and serialisation, text extraction, sanitisation, and the configuration surface for key extensions.

## Contributing

1. Install the .NET 8 SDK.
2. Restore dependencies with `dotnet restore` at the repository root.
3. Build with `dotnet build` (or open `Tiptap.sln` in your IDE).
4. Run `dotnet test` to ensure the suite passes.
5. Submit a pull request with tests if you add functionality.

---

This port intentionally mirrors the API of the PHP package. If you discover behaviour that differs from either the PHP or JavaScript reference implementations, please open an issue so we can keep the ecosystems aligned.

## License
See [LICENSE.md](LICENSE.md) for details.