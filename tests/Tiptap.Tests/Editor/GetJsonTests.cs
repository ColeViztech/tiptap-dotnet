using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.Editor;

public class GetJsonTests
{
    [Fact]
    public void GetJsonReturnsJson()
    {
        var json = new EditorClass()
            .SetContent("<p>Example</p>")
            .GetJSON();

        Assert.Equal("{\"type\":\"doc\",\"content\":[{\"type\":\"paragraph\",\"content\":[{\"type\":\"text\",\"text\":\"Example\"}]}]}", json);
    }
}
