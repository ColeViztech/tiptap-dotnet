using System.Linq;
using Tiptap.Core.Models;

namespace Tiptap.Core.Core;

public abstract class Mark : Extension
{
    protected Mark(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override int Priority => 100;

    public virtual IDictionary<string, AttributeConfiguration> AddAttributes()
        => new Dictionary<string, AttributeConfiguration>();

    public virtual object? RenderHTML(ProseMirrorMark mark)
        => null;

    public virtual IEnumerable<ParseRule> ParseHTML(object? context = null)
        => Enumerable.Empty<ParseRule>();
}
