using Robust.Client.Graphics;

namespace Content.Game.StyleSheet.StyleBox;

[Serializable, DataDefinition]
public sealed partial class StyleBoxEmptyData : Game.StyleSheet.StyleBox.StyleBoxData
{
    public static implicit operator StyleBoxEmpty(StyleBoxEmptyData data)
    {
        var box = new StyleBoxEmpty();
        data.SetBaseParam(ref box);
        return box;
    }
}