using System;
using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Models;
using Tiptap.Core.Nodes;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class MentionDomSerializerTests
{
    [Fact]
    public void UserMentionIsSerializedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Mention(),
            }));

        var document = Doc(
            Paragraph(
                Text("Hey "),
                Mention(new Dictionary<string, object?>
                {
                    ["id"] = 123,
                }),
                Text(", was geht?")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>Hey <span data-id=\"123\" data-type=\"mention\"></span>, was geht?</p>", result);
    }

    [Fact]
    public void LabelCanBeCustomized()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Mention(new Dictionary<string, object?>
                {
                    ["renderLabel"] = (Func<ProseMirrorNode, string?>)(_ => "@Philipp"),
                }),
            }));

        var document = Doc(
            Paragraph(
                Text("Hey "),
                Mention(new Dictionary<string, object?>
                {
                    ["id"] = 123,
                }),
                Text(", was geht?")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>Hey <span data-id=\"123\" data-type=\"mention\">@Philipp</span>, was geht?</p>", result);
    }
}
