using System;
using System.Collections.ObjectModel;
using System.Linq;
using Tiptap.Core.Models;

namespace Tiptap.Core.Core;

public class Schema
{
    private readonly Dictionary<string, Dictionary<string, AttributeConfiguration>> _globalAttributes = new();

    public IReadOnlyList<Extension> AllExtensions { get; }

    public IReadOnlyList<Node> Nodes { get; }

    public IReadOnlyList<Mark> Marks { get; }

    public IReadOnlyList<Extension> Extensions { get; }

    public Node? DefaultNode { get; }

    public Node? TopNode { get; }

    public Schema(IEnumerable<Extension> extensions)
    {
        var loaded = LoadExtensions(extensions).ToList();
        loaded.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        AllExtensions = new ReadOnlyCollection<Extension>(loaded);
        Nodes = new ReadOnlyCollection<Node>(loaded.OfType<Node>().ToList());
        Marks = new ReadOnlyCollection<Mark>(loaded.OfType<Mark>().ToList());
        Extensions = new ReadOnlyCollection<Extension>(loaded);

        DefaultNode = Nodes.FirstOrDefault();
        TopNode = Nodes.FirstOrDefault(node => node.IsTopNode) ?? DefaultNode;
    }

    private IEnumerable<Extension> LoadExtensions(IEnumerable<Extension> extensions)
    {
        var result = new List<Extension>();
        var processedTypes = new HashSet<Type>();

        foreach (var extension in extensions)
        {
            if (processedTypes.Add(extension.GetType()))
            {
                result.Add(extension);
            }

            foreach (var global in extension.AddGlobalAttributes())
            {
                foreach (var type in global.Types)
                {
                    if (!_globalAttributes.TryGetValue(type, out var map))
                    {
                        map = new Dictionary<string, AttributeConfiguration>();
                        _globalAttributes[type] = map;
                    }

                    foreach (var attribute in global.Attributes)
                    {
                        map[attribute.Key] = attribute.Value;
                    }
                }
            }

            var additional = extension.AddExtensions()?.ToList() ?? new List<Extension>();
            if (additional.Count > 0)
            {
                result.AddRange(LoadExtensions(additional));
            }
        }

        return result;
    }

    public ProseMirrorDocument Apply(ProseMirrorDocument document)
    {
        if (document.Content == null)
        {
            return document;
        }

        document.Content = document.Content
            .Select(FilterNodeMarks)
            .ToList();

        return document;
    }

    private ProseMirrorNode FilterNodeMarks(ProseMirrorNode node)
    {
        foreach (var extension in AllExtensions)
        {
            if (node.Type != extension.Name)
            {
                continue;
            }

            if (extension is Node nodeExtension)
            {
                if (nodeExtension.Marks == string.Empty)
                {
                    node.Marks = null;
                    if (node.Content != null)
                    {
                        node.Content = node.Content.Select(FilterNodeMarks).ToList();
                    }
                }
            }

            break;
        }

        return node;
    }

    public IDictionary<string, AttributeConfiguration> GetAttributeConfigurations(Extension extension)
    {
        var attributes = new Dictionary<string, AttributeConfiguration>();

        if (_globalAttributes.TryGetValue(extension.Name, out var global))
        {
            foreach (var item in global)
            {
                attributes[item.Key] = item.Value;
            }
        }

        switch (extension)
        {
            case Node node:
                foreach (var attribute in node.AddAttributes())
                {
                    attributes[attribute.Key] = attribute.Value;
                }

                break;
            case Mark mark:
                foreach (var attribute in mark.AddAttributes())
                {
                    attributes[attribute.Key] = attribute.Value;
                }

                break;
        }

        return attributes;
    }
}
