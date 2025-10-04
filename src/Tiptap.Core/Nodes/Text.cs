using Tiptap.Core.Core;

namespace Tiptap.Core.Nodes;

public class Text : Node
{
    public Text(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "text";

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "#text",
        };
    }
}
