using System.Linq;
using Tiptap.Core.Models;

namespace Tiptap.Core.Core;

public abstract class Node : Extension
{
    protected Node(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override int Priority => 100;

    public virtual bool IsTopNode => false;

    public virtual string Marks => "_";

    public virtual IDictionary<string, AttributeConfiguration> AddAttributes()
        => new Dictionary<string, AttributeConfiguration>();

    public virtual IEnumerable<ParseRule> ParseHTML(object? context = null)
        => Enumerable.Empty<ParseRule>();

    public virtual object? RenderHTML(ProseMirrorNode node)
        => null;

    public virtual string? RenderText(ProseMirrorNode node)
        => node.Text;
}
