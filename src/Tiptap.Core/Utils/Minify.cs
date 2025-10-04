using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tiptap.Core.Utils;

public class Minify
{
    private string? _replacementHash;
    private readonly Dictionary<string, string> _placeholders = new();
    private string _html = string.Empty;

    public string Process(string html)
    {
        _html = html.Replace("\r\n", "\n").Trim();

        var hashSource = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        _replacementHash = "MINIFYHTML" + ComputeHash(hashSource);

        _html = Regex.Replace(
            _html,
            "\\s*<pre(\\b[^>]*?>[\\s\\S]*?<\\/pre>)\\s*",
            match => RemovePre(match.Groups[1].Value),
            RegexOptions.IgnoreCase | RegexOptions.Multiline
        );

        _html = Regex.Replace(_html, "^\\s+|\\s+$", string.Empty, RegexOptions.Multiline);

        _html = Regex.Replace(
            _html,
            "\\s+(<\\/?(?:area|article|aside|base(?:font)?|blockquote|body|canvas|caption|center|col(?:group)?|dd|dir|div|dl|dt|fieldset|figcaption|figure|footer|form|frame(?:set)?|h[1-6]|head|header|hgroup|hr|html|legend|li|link|main|map|menu|meta|nav|ol|opt(?:group|ion)|output|p|param|section|t(?:able|body|head|d|h|r|foot|itle)|ul|video)\\b[^>]*>)",
            "$1",
            RegexOptions.IgnoreCase
        );

        foreach (var placeholder in _placeholders)
        {
            _html = _html.Replace(placeholder.Key, placeholder.Value);
        }

        return _html;
    }

    private string RemovePre(string content)
    {
        return ReservePlace("<pre" + content);
    }

    private string ReservePlace(string content)
    {
        if (_replacementHash == null)
        {
            throw new InvalidOperationException("Replacement hash is not initialised.");
        }

        var placeholder = $"%{_replacementHash}{_placeholders.Count}%";
        _placeholders[placeholder] = content;

        return placeholder;
    }

    private static string ComputeHash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = md5.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
