using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Cinka.Game.Dialog.Data;

[DataDefinition]
public abstract partial class DialogStyle
{
    [DataField(required:true)] public ResPath BackgroundPath = default!;
    
    [DataField] public Box2 BackgroundPatchMargin = default;
    
    [DataField]
    public Color BackgroundModulate = Color.White;

    [DataField] public Color TextColor = Color.Black;

    [DataField] public Control.HAlignment HorizontalAlignment = Control.HAlignment.Left;

    [DataField] public Box2 Margin = new Box2();
}