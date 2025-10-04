using Tiptap.Core.Core;

namespace Tiptap.Core.Nodes;

public class Document : Node
{
    public Document(IDictionary<string, object?>? options = null)
        : base(options)
    {
    }

    public override string Name => "doc";

    public override bool IsTopNode => true;
}
