using System.Net;
using Tiptap.Core.Models;

namespace Tiptap.Core.Core;

public class TextSerializer
{
    private readonly Schema _schema;
    private readonly string _blockSeparator;

    public TextSerializer(Schema schema, IDictionary<string, object?>? configuration = null)
    {
        _schema = schema;
        var defaults = new Dictionary<string, object?>
        {
            ["blockSeparator"] = "\n\n",
        };

        if (configuration != null)
        {
            foreach (var option in configuration)
            {
                defaults[option.Key] = option.Value;
            }
        }

        _blockSeparator = defaults.TryGetValue("blockSeparator", out var separator) && separator is string value
            ? value
            : "\n\n";
    }

    public string Process(object value)
    {
        var document = Serialization.ToDocument(value);
        if (document.Content == null)
        {
            return string.Empty;
        }

        var blocks = new List<string>();
        foreach (var node in document.Content)
        {
            var rendered = RenderNode(node);
            if (!string.IsNullOrEmpty(rendered))
            {
                blocks.Add(rendered);
            }
        }

        return string.Join(_blockSeparator, blocks);
    }

    private string RenderNode(ProseMirrorNode node)
    {
        var fragments = new List<string>();

        if (node.Content != null)
        {
            foreach (var nested in node.Content)
            {
                var rendered = RenderNode(nested);
                if (!string.IsNullOrEmpty(rendered))
                {
                    fragments.Add(rendered);
                }
            }
        }
        else if (!string.IsNullOrEmpty(node.Text))
        {
            fragments.Add(WebUtility.HtmlEncode(node.Text));
        }

        return string.Join(_blockSeparator, fragments);
    }
}
