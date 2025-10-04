using System.Linq;
using Tiptap.Core.Core;
using Tiptap.Core.Marks;
using Tiptap.Core.Nodes;

namespace Tiptap.Core.Extensions;

public class StarterKit : Extension
{
    public StarterKit(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "starterKit";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["document"] = new Dictionary<string, object?>(),
            ["blockquote"] = new Dictionary<string, object?>(),
            ["bulletList"] = new Dictionary<string, object?>(),
            ["codeBlock"] = new Dictionary<string, object?>(),
            ["hardBreak"] = new Dictionary<string, object?>(),
            ["heading"] = new Dictionary<string, object?>(),
            ["horizontalRule"] = new Dictionary<string, object?>(),
            ["listItem"] = new Dictionary<string, object?>(),
            ["orderedList"] = new Dictionary<string, object?>(),
            ["paragraph"] = new Dictionary<string, object?>(),
            ["text"] = new Dictionary<string, object?>(),
            ["bold"] = new Dictionary<string, object?>(),
            ["code"] = new Dictionary<string, object?>(),
            ["italic"] = new Dictionary<string, object?>(),
            ["strike"] = new Dictionary<string, object?>(),
        };
    }

    private bool IsEnabled(string key)
    {
        if (!Options.TryGetValue(key, out var value))
        {
            return true;
        }

        return value switch
        {
            bool boolean => boolean,
            string text when bool.TryParse(text, out var parsed) => parsed,
            _ => true,
        };
    }

    private IDictionary<string, object?>? GetExtensionOptions(string key)
    {
        if (!Options.TryGetValue(key, out var value))
        {
            return null;
        }

        return value switch
        {
            IDictionary<string, object?> dictionary => dictionary,
            IEnumerable<KeyValuePair<string, object?>> pairs => pairs.ToDictionary(pair => pair.Key, pair => pair.Value),
            _ => null,
        };
    }

    public override IEnumerable<Extension> AddExtensions()
    {
        if (IsEnabled("document"))
        {
            yield return new Document(GetExtensionOptions("document"));
        }

        if (IsEnabled("blockquote"))
        {
            yield return new Blockquote(GetExtensionOptions("blockquote"));
        }

        if (IsEnabled("bulletList"))
        {
            yield return new BulletList(GetExtensionOptions("bulletList"));
        }

        if (IsEnabled("codeBlock"))
        {
            yield return new CodeBlock(GetExtensionOptions("codeBlock"));
        }

        if (IsEnabled("hardBreak"))
        {
            yield return new HardBreak(GetExtensionOptions("hardBreak"));
        }

        if (IsEnabled("heading"))
        {
            yield return new Heading(GetExtensionOptions("heading"));
        }

        if (IsEnabled("horizontalRule"))
        {
            yield return new HorizontalRule(GetExtensionOptions("horizontalRule"));
        }

        if (IsEnabled("listItem"))
        {
            yield return new ListItem(GetExtensionOptions("listItem"));
        }

        if (IsEnabled("orderedList"))
        {
            yield return new OrderedList(GetExtensionOptions("orderedList"));
        }

        if (IsEnabled("paragraph"))
        {
            yield return new Paragraph(GetExtensionOptions("paragraph"));
        }

        if (IsEnabled("text"))
        {
            yield return new Text(GetExtensionOptions("text"));
        }

        if (IsEnabled("bold"))
        {
            yield return new Bold(GetExtensionOptions("bold"));
        }

        if (IsEnabled("code"))
        {
            yield return new Code(GetExtensionOptions("code"));
        }

        if (IsEnabled("italic"))
        {
            yield return new Italic(GetExtensionOptions("italic"));
        }

        if (IsEnabled("strike"))
        {
            yield return new Strike(GetExtensionOptions("strike"));
        }
    }
}
