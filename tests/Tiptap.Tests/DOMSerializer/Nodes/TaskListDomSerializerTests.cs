using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class TaskListDomSerializerTests
{
    private static EditorClass CreateEditor()
    {
        return new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TaskList(),
                new TaskItem(),
            }));
    }

    [Fact]
    public void TaskListGetsRenderedCorrectly()
    {
        var editor = CreateEditor();

        var document = Doc(
            TaskList(
                TaskItem(
                    Paragraph(
                        Text("Example Text")))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<ul data-type=\"taskList\"><li data-checked=\"false\" data-type=\"taskItem\"><div data-checked=\"false\"><p>Example Text</p></div></li></ul>", result);
    }

    [Fact]
    public void TaskItemStatusIsRenderedCorrectly()
    {
        var editor = CreateEditor();

        var document = Doc(
            TaskList(
                TaskItem(
                    new Dictionary<string, object?>
                    {
                        ["checked"] = true,
                    },
                    Paragraph(
                        Text("Example Text")))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<ul data-type=\"taskList\"><li data-checked=\"true\" data-type=\"taskItem\"><div data-checked=\"true\"><p>Example Text</p></div></li></ul>", result);
    }
}
