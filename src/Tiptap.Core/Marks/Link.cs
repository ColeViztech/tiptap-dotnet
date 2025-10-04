using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Marks;

public class Link : Mark
{
    private static readonly Regex AttributeWhitespaceRegex = new("[\\x00-\\x20\\u00A0\\u1680\\u180E\\u2000-\\u2029\\u205F\\u3000\\uFFFD]", RegexOptions.Compiled);

    public Link(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "link";

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>
            {
                ["target"] = "_blank",
                ["rel"] = "noopener noreferrer nofollow",
            },
            ["allowedProtocols"] = new List<string>
            {
                "http", "https", "ftp", "ftps", "mailto", "tel", "callto", "sms", "cid", "xmpp",
            },
            ["isAllowedUri"] = (Func<string?, bool>)(uri => IsAllowedUri(uri)),
        };
    }

    private Func<string?, bool> GetIsAllowedUri()
    {
        if (Options.TryGetValue("isAllowedUri", out var value))
        {
            switch (value)
            {
                case Func<string?, bool> func:
                    return func;
                case Delegate del:
                    return uri =>
                    {
                        var result = del.DynamicInvoke(uri);
                        return result is bool boolean && boolean;
                    };
            }
        }

        return IsAllowedUri;
    }

    private IReadOnlyList<string> GetAllowedProtocols()
    {
        if (Options.TryGetValue("allowedProtocols", out var value))
        {
            switch (value)
            {
                case IEnumerable<string> stringEnumerable:
                    return stringEnumerable.ToList();
                case IEnumerable<object?> objectEnumerable:
                    return objectEnumerable
                        .Select(item => item?.ToString())
                        .Where(text => !string.IsNullOrEmpty(text))
                        .Select(text => text!)
                        .ToList();
            }
        }

        return new List<string>
        {
            "http", "https", "ftp", "ftps", "mailto", "tel", "callto", "sms", "cid", "xmpp",
        };
    }

    public bool IsAllowedUri(string? uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return true;
        }

        var sanitised = AttributeWhitespaceRegex.Replace(uri, string.Empty);
        var protocols = GetAllowedProtocols();
        var pattern = $"^(?:(?:{string.Join("|", protocols.Select(Regex.Escape))}):|[^a-z]|[a-z0-9+.-]+(?:[^a-z+.-:]|$))";

        return Regex.IsMatch(sanitised, pattern, RegexOptions.IgnoreCase);
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        var isAllowed = GetIsAllowedUri();

        yield return new ParseRule
        {
            Tag = "a[href]",
            GetAttrs = parameter =>
            {
                if (parameter is IElement element)
                {
                    var href = element.GetAttribute("href") ?? string.Empty;
                    if (href == string.Empty || !isAllowed(href))
                    {
                        return false;
                    }
                }

                return null;
            },
        };
    }

    public override IDictionary<string, AttributeConfiguration> AddAttributes()
    {
        return new Dictionary<string, AttributeConfiguration>
        {
            ["href"] = new AttributeConfiguration(),
            ["target"] = new AttributeConfiguration(),
            ["rel"] = new AttributeConfiguration(),
            ["class"] = new AttributeConfiguration(),
        };
    }

    public override object? RenderHTML(ProseMirrorMark mark)
    {
        var isAllowed = GetIsAllowedUri();
        var markAttributes = new Dictionary<string, object?>();

        if (mark.Attrs != null)
        {
            foreach (var (key, value) in mark.Attrs)
            {
                if (value != null)
                {
                    markAttributes[key] = value;
                }
            }
        }

        if (markAttributes.TryGetValue("href", out var hrefValue))
        {
            var href = hrefValue?.ToString();
            if (!isAllowed(href))
            {
                markAttributes["href"] = string.Empty;
            }
        }

        var attributes = HtmlUtilities.MergeAttributes(this.GetHtmlAttributes(), markAttributes);

        return new object?[]
        {
            "a",
            attributes,
            0,
        };
    }
}
