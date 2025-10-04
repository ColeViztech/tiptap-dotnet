using System.Linq;

namespace Tiptap.Core.Core;

public abstract class Extension
{
    public virtual string Name => GetType().Name;

    public virtual int Priority => 100;

    public IReadOnlyDictionary<string, object?> Options { get; }

    protected Extension(IDictionary<string, object?>? options = null)
    {
        var defaults = AddOptions();
        var merged = new Dictionary<string, object?>(defaults);

        if (options != null)
        {
            foreach (var option in options)
            {
                merged[option.Key] = option.Value;
            }
        }

        Options = merged;
    }

    protected virtual IDictionary<string, object?> AddOptions()
        => new Dictionary<string, object?>();

    public virtual IEnumerable<GlobalAttributeConfiguration> AddGlobalAttributes()
        => Enumerable.Empty<GlobalAttributeConfiguration>();

    public virtual IEnumerable<Extension> AddExtensions()
        => Enumerable.Empty<Extension>();
}

public record GlobalAttributeConfiguration(
    IEnumerable<string> Types,
    IDictionary<string, AttributeConfiguration> Attributes
);

public class AttributeConfiguration
{
    public bool? Rendered { get; init; }

    public object? Default { get; init; }

    public Func<object?, IDictionary<string, object?>?>? RenderHTML { get; init; }

    public Func<object?, object?>? ParseHTML { get; init; }
}
