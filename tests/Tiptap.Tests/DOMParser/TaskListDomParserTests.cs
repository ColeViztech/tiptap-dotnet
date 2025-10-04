using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Nodes;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class TaskListDomParserTests
{
    [Fact]
    public void TaskListGetsParsedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TaskList(),
                new TaskItem(),
            }));

        var result = editor
            .SetContent("<ul data-type=\"taskList\"><li data-type=\"taskItem\"><p>Example Text</p></li></ul>")
            .GetDocument();

        var expected = Doc(
            TaskList(
                TaskItem(
                    Paragraph(
                        Text("Example Text")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void BulletListsAreStillParsedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TaskList(),
                new TaskItem(),
            }));

        var result = editor
            .SetContent("<ul><li><p>Example Text</p></li></ul>")
            .GetDocument();

        var expected = Doc(
            BulletList(
                ListItem(
                    Paragraph(
                        Text("Example Text")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
