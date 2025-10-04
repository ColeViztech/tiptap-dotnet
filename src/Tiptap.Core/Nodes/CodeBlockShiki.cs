using System.Linq;
using System.Net;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class CodeBlockShiki : CodeBlock
{
    public CodeBlockShiki(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["languageClassPrefix"] = "language-",
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
            ["defaultLanguage"] = "html",
            ["theme"] = "nord",
            ["guessLanguage"] = true,
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        var code = node.Content?.FirstOrDefault()?.Text ?? string.Empty;
        var language = node.Attrs != null && node.Attrs.TryGetValue("language", out var value)
            ? value?.ToString()
            : null;

        if (string.IsNullOrWhiteSpace(language))
        {
            language = this.GetStringOption("defaultLanguage", "html");
        }

        var prefix = this.GetStringOption("languageClassPrefix", "language-");
        var mergedAttributes = HtmlUtilities.MergeAttributes(
            new Dictionary<string, object?>
            {
                ["class"] = !string.IsNullOrWhiteSpace(language)
                    ? prefix + language
                    : null,
            },
            this.GetHtmlAttributes());

        var renderedAttributes = HtmlUtilities.RenderAttributes(mergedAttributes);
        var content = $"<pre><code{renderedAttributes}>{WebUtility.HtmlEncode(code)}</code></pre>";

        return new Dictionary<string, object?>
        {
            ["content"] = content,
        };
    }
}
