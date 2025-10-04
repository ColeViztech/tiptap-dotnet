using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class TableDomSerializerTests
{
    private static EditorClass CreateEditor()
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Table(),
                new TableRow(),
                new TableCell(),
                new TableHeader(),
            }));
    }

    [Fact]
    public void SimpleTableNodeGetsRenderedCorrectly()
    {
        var editor = CreateEditor();

        var document = Doc(
            Table(
                TableRow(
                    TableHeader(
                        Paragraph(
                            Text("text in header cell"))))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<table><tbody><tr><th><p>text in header cell</p></th></tr></tbody></table>", result);
    }

    [Fact]
    public void TableNodeGetsRenderedCorrectly()
    {
        var editor = CreateEditor();

        var document = Doc(
            Table(
                TableRow(
                    TableHeader(
                        Paragraph(
                            Text("text in header cell"))),
                    TableHeader(
                        new Dictionary<string, object?>
                        {
                            ["colspan"] = 2,
                            ["colwidth"] = new object?[] { 100, 0 },
                        },
                        Paragraph(
                            Text("text in header cell with colspan 2")))),
                TableRow(
                    TableCell(
                        new Dictionary<string, object?>
                        {
                            ["rowspan"] = 2,
                        },
                        Paragraph(
                            Text("paragraph 1 in cell with rowspan 2")),
                        Paragraph(
                            Text("paragraph 2 in cell with rowspan 2"))),
                    TableCell(
                        Paragraph(
                            Text("foo"))),
                    TableCell(
                        Paragraph(
                            Text("bar")))),
                TableRow(
                    TableCell(
                        Paragraph(
                            Text("foo"))),
                    TableCell(
                        Paragraph(
                            Text("bar"))))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        var expected = "<table><tbody>"
            + "<tr>"
            + "<th><p>text in header cell</p></th>"
            + "<th colspan=\"2\" data-colwidth=\"100,0\"><p>text in header cell with colspan 2</p></th>"
            + "</tr>"
            + "<tr>"
            + "<td rowspan=\"2\"><p>paragraph 1 in cell with rowspan 2</p><p>paragraph 2 in cell with rowspan 2</p></td>"
            + "<td><p>foo</p></td>"
            + "<td><p>bar</p></td>"
            + "</tr>"
            + "<tr>"
            + "<td><p>foo</p></td>"
            + "<td><p>bar</p></td>"
            + "</tr>"
            + "</tbody></table>";

        Assert.Equal(expected, result);
    }
}
