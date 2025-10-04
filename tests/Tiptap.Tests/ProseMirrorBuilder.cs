using System.Collections.Generic;
using System.Linq;
using Tiptap.Core.Models;

namespace Tiptap.Tests;

internal static class ProseMirrorBuilder
{
    public static ProseMirrorDocument Doc(params ProseMirrorNode[] content)
    {
        return new ProseMirrorDocument
        {
            Type = "doc",
            Content = content.Length > 0 ? content.ToList() : null,
        };
    }

    public static ProseMirrorNode Paragraph(params ProseMirrorNode[] content)
    {
        return CreateNode("paragraph", null, content);
    }

    public static ProseMirrorNode Paragraph(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("paragraph", attrs, content);
    }

    public static ProseMirrorNode Text(string text, params ProseMirrorMark[] marks)
    {
        return new ProseMirrorNode
        {
            Type = "text",
            Text = text,
            Marks = marks.Length > 0 ? marks.ToList() : null,
        };
    }

    public static ProseMirrorNode Blockquote(params ProseMirrorNode[] content)
    {
        return CreateNode("blockquote", null, content);
    }

    public static ProseMirrorNode BulletList(params ProseMirrorNode[] content)
    {
        return CreateNode("bulletList", null, content);
    }

    public static ProseMirrorNode CodeBlock(params ProseMirrorNode[] content)
    {
        return CreateNode("codeBlock", null, content);
    }

    public static ProseMirrorNode CodeBlock(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("codeBlock", attrs, content);
    }

    public static ProseMirrorNode Details(params ProseMirrorNode[] content)
    {
        return CreateNode("details", null, content);
    }

    public static ProseMirrorNode Details(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("details", attrs, content);
    }

    public static ProseMirrorNode DetailsContent(params ProseMirrorNode[] content)
    {
        return CreateNode("detailsContent", null, content);
    }

    public static ProseMirrorNode DetailsContent(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("detailsContent", attrs, content);
    }

    public static ProseMirrorNode DetailsSummary(params ProseMirrorNode[] content)
    {
        return CreateNode("detailsSummary", null, content);
    }

    public static ProseMirrorNode HardBreak(params ProseMirrorMark[] marks)
    {
        return new ProseMirrorNode
        {
            Type = "hardBreak",
            Marks = marks.Length > 0 ? marks.ToList() : null,
        };
    }

    public static ProseMirrorNode Heading(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("heading", attrs, content);
    }

    public static ProseMirrorNode HorizontalRule()
    {
        return CreateNode("horizontalRule", null);
    }

    public static ProseMirrorNode Image(IDictionary<string, object?> attrs)
    {
        return CreateNode("image", attrs);
    }

    public static ProseMirrorNode ListItem(params ProseMirrorNode[] content)
    {
        return CreateNode("listItem", null, content);
    }

    public static ProseMirrorNode TaskList(params ProseMirrorNode[] content)
    {
        return CreateNode("taskList", null, content);
    }

    public static ProseMirrorNode TaskItem(params ProseMirrorNode[] content)
    {
        return CreateNode("taskItem", null, content);
    }

    public static ProseMirrorNode TaskItem(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("taskItem", attrs, content);
    }

    public static ProseMirrorNode Mention(IDictionary<string, object?> attrs)
    {
        return CreateNode("mention", attrs);
    }

    public static ProseMirrorNode OrderedList(params ProseMirrorNode[] content)
    {
        return CreateNode("orderedList", null, content);
    }

    public static ProseMirrorNode OrderedList(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("orderedList", attrs, content);
    }

    public static ProseMirrorNode Table(params ProseMirrorNode[] content)
    {
        return CreateNode("table", null, content);
    }

    public static ProseMirrorNode TableCell(params ProseMirrorNode[] content)
    {
        return CreateNode("tableCell", null, content);
    }

    public static ProseMirrorNode TableCell(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("tableCell", attrs, content);
    }

    public static ProseMirrorNode TableHeader(params ProseMirrorNode[] content)
    {
        return CreateNode("tableHeader", null, content);
    }

    public static ProseMirrorNode TableHeader(IDictionary<string, object?> attrs, params ProseMirrorNode[] content)
    {
        return CreateNode("tableHeader", attrs, content);
    }

    public static ProseMirrorNode TableRow(params ProseMirrorNode[] content)
    {
        return CreateNode("tableRow", null, content);
    }

    public static ProseMirrorMark Mark(string type, IDictionary<string, object?>? attrs = null)
    {
        return new ProseMirrorMark
        {
            Type = type,
            Attrs = attrs != null ? new Dictionary<string, object?>(attrs) : null,
        };
    }

    private static ProseMirrorNode CreateNode(string type, IDictionary<string, object?>? attrs, params ProseMirrorNode[] content)
    {
        return new ProseMirrorNode
        {
            Type = type,
            Attrs = attrs != null ? new Dictionary<string, object?>(attrs) : null,
            Content = content.Length > 0 ? content.ToList() : null,
        };
    }
}
