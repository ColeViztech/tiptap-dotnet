using System.Text.Json;
using System.Linq;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Models;
using CoreJsonSerializer = Tiptap.Core.Core.JsonSerializer;
using CoreDomSerializer = Tiptap.Core.Core.DomSerializer;
using CoreTextSerializer = Tiptap.Core.Core.TextSerializer;

namespace Tiptap.Core;

public class Editor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private ProseMirrorDocument? _document;

    public Schema Schema { get; }

    public IReadOnlyList<Extension> Extensions { get; }

    public EditorOptions Options { get; }

    public Editor(EditorOptions? options = null)
    {
        Options = options ?? new EditorOptions();

        var extensions = Options.Extensions?.ToList() ?? new List<Extension>();
        if (extensions.Count == 0)
        {
            extensions.Add(new StarterKit());
        }

        Extensions = extensions.AsReadOnly();
        Schema = new Schema(Extensions);

        _document = Schema.Apply(CreateEmptyDocument());

        if (Options.Content != null)
        {
            SetContent(Options.Content);
        }
    }

    public Editor SetContent(object? value)
    {
        if (value == null)
        {
            _document = Schema.Apply(CreateEmptyDocument());
            return this;
        }

        var kind = DetermineContentKind(value);

        ProseMirrorDocument document = kind switch
        {
            ContentKind.Html => ParseHtml((string)value),
            ContentKind.Json => ParseJson((string)value),
            ContentKind.Document => Serialization.ToDocument(value).Clone(),
            _ => throw new InvalidOperationException("Unsupported content type."),
        };

        document.Type ??= Schema.TopNode?.Name;
        _document = Schema.Apply(document);

        return this;
    }

    public ProseMirrorDocument GetDocument()
    {
        return EnsureDocument().Clone();
    }

    public string GetJSON()
    {
        return new CoreJsonSerializer().Process(EnsureDocument());
    }

    public string GetHTML()
    {
        return new CoreDomSerializer(Schema).Process(EnsureDocument());
    }

    public string GetText(IDictionary<string, object?>? configuration = null)
    {
        return new CoreTextSerializer(Schema, configuration).Process(EnsureDocument());
    }

    public object? Sanitize(object? value)
    {
        if (value == null)
        {
            return SetContent((object?)null).GetDocument();
        }

        return DetermineContentKind(value) switch
        {
            ContentKind.Html => SetContent(value).GetHTML(),
            ContentKind.Json => SetContent(value).GetJSON(),
            ContentKind.Document => SetContent(value).GetDocument(),
            _ => throw new InvalidOperationException("Unsupported content type."),
        };
    }

    public string GetContentType(object value)
    {
        return DetermineContentKind(value) switch
        {
            ContentKind.Html => "HTML",
            ContentKind.Json => "JSON",
            ContentKind.Document => "Array",
            _ => throw new InvalidOperationException("Unsupported content type."),
        };
    }

    public Editor Descendants(Action<ProseMirrorNode> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var document = EnsureDocument().Clone();

        WalkThroughDocument(document, action);

        SetContent(document);

        return this;
    }

    private void WalkThroughDocument(ProseMirrorDocument document, Action<ProseMirrorNode> action)
    {
        var root = new ProseMirrorNode
        {
            Type = document.Type,
            Content = document.Content,
            AdditionalData = document.AdditionalData != null
                ? new Dictionary<string, object?>(document.AdditionalData)
                : null,
        };

        WalkThroughNodes(root, action);

        document.Content = root.Content;
        document.AdditionalData = root.AdditionalData != null
            ? new Dictionary<string, object?>(root.AdditionalData)
            : null;
    }

    private void WalkThroughNodes(ProseMirrorNode node, Action<ProseMirrorNode> action)
    {
        if (string.Equals(node.Type, "text", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        action(node);

        if (node.Content == null)
        {
            return;
        }

        foreach (var child in node.Content)
        {
            WalkThroughNodes(child, action);
        }
    }

    private ContentKind DetermineContentKind(object value)
    {
        return value switch
        {
            string text => IsJson(text) ? ContentKind.Json : ContentKind.Html,
            _ => ContentKind.Document,
        };
    }

    private static bool IsJson(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            using var _ = JsonDocument.Parse(value);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private ProseMirrorDocument ParseHtml(string value)
    {
        return new DomParser(Schema).Process(value);
    }

    private static ProseMirrorDocument ParseJson(string value)
    {
        return System.Text.Json.JsonSerializer.Deserialize<ProseMirrorDocument>(value, JsonOptions)
               ?? new ProseMirrorDocument();
    }

    private ProseMirrorDocument EnsureDocument()
    {
        if (_document == null)
        {
            _document = Schema.Apply(CreateEmptyDocument());
        }

        return _document;
    }

    private ProseMirrorDocument CreateEmptyDocument()
    {
        return new ProseMirrorDocument
        {
            Type = Schema.TopNode?.Name,
            Content = new List<ProseMirrorNode>(),
        };
    }

    private enum ContentKind
    {
        Html,
        Json,
        Document,
    }
}

public record EditorOptions(object? Content = null, IEnumerable<Extension>? Extensions = null);
