using Tiptap.Core.Core;
using Tiptap.Core.Models;
using Tiptap.Core.Utils;

namespace Tiptap.Core.Nodes;

public class TaskList : Node
{
    public TaskList(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "taskList";

    public override int Priority => 1000;

    protected override IDictionary<string, object?> AddOptions()
    {
        return new Dictionary<string, object?>
        {
            ["HTMLAttributes"] = new Dictionary<string, object?>(),
        };
    }

    public override IEnumerable<ParseRule> ParseHTML(object? context = null)
    {
        yield return new ParseRule
        {
            Tag = "ul[data-type=\"taskList\"]",
        };
    }

    public override object? RenderHTML(ProseMirrorNode node)
    {
        return new object?[]
        {
            "ul",
            HtmlUtilities.MergeAttributes(
                this.GetHtmlAttributes(),
                new Dictionary<string, object?>
                {
                    ["data-type"] = "taskList",
                }),
            0,
        };
    }
}
