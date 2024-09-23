using System.Numerics;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.Utility;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using IRsiStateLike = Robust.Client.Graphics.IRsiStateLike;
using StyleBoxTexture = Robust.Client.Graphics.StyleBoxTexture;
using Texture = Robust.Client.Graphics.Texture;

namespace Content.Game.StyleSheet.StyleBox;

[Serializable, DataDefinition]
public sealed partial class StyleBoxTextureData : Game.StyleSheet.StyleBox.StyleBoxData
{
    [DataField] public SpriteSpecifier Texture;
    
    /// <summary>
    /// Left expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float ExpandMarginLeft;
    /// <summary>
    /// Top expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float ExpandMarginTop;

    /// <summary>
    /// Bottom expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float ExpandMarginBottom ;

    /// <summary>
    /// Right expansion size, in virtual pixels.
    /// </summary>
    /// <remarks>
    /// This expands the size of the area where the patches get drawn. This will cause the drawn texture to
    /// extend beyond the box passed to the <see cref="StyleBox.Draw"/> function. This is not affected by
    /// <see cref="TextureScale"/>.
    /// </remarks>
    [DataField] public float ExpandMarginRight;

    [DataField] public StyleBoxTexture.StretchMode Mode = StyleBoxTexture.StretchMode.Stretch;

    /// <summary>
    /// Distance of the left patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float PatchMarginLeft;
    /// <summary>
    /// Distance of the right patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float PatchMarginRight;

    /// <summary>
    /// Distance of the top patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float PatchMarginTop;

    /// <summary>
    /// Distance of the bottom patch margin from the image. In texture space.
    /// The size of this patch in virtual pixels can be obtained by scaling this with <see cref="TextureScale"/>.
    /// </summary>
    [DataField] public float PatchMarginBottom;

    [DataField] public Thickness? PatchMargin;
    [DataField] public Thickness? ExpandMargin;

    [DataField] public Color Modulate = Color.White;
    
    /// <summary>
    /// Additional scaling to use when drawing the texture.
    /// </summary>
    [DataField] public Vector2 TextureScale  = Vector2.One;
    
    public StyleBoxTexture GetStyleboxTexture(IDependencyCollection dependencyCollection)
    {
        var styleBox = new StyleBoxTexture();
        SetBaseParam(ref styleBox);
        styleBox.Texture = RsiStateLike(Texture, dependencyCollection).Default;
        styleBox.Mode = Mode;
        styleBox.Modulate = Modulate;
        styleBox.TextureScale = TextureScale;

        if (ExpandMargin is null)
        {
            styleBox.ExpandMarginBottom = ExpandMarginBottom;
            styleBox.ExpandMarginTop = ExpandMarginTop;
            styleBox.ExpandMarginRight = ExpandMarginRight;
            styleBox.ExpandMarginLeft = ExpandMarginLeft;
        }
        else
        {
            styleBox.ExpandMarginBottom = ExpandMargin.Value.Bottom;
            styleBox.ExpandMarginTop = ExpandMargin.Value.Top;
            styleBox.ExpandMarginRight = ExpandMargin.Value.Right;
            styleBox.ExpandMarginLeft = ExpandMargin.Value.Left;
        }

        if (PatchMargin is null)
        {
            styleBox.PatchMarginBottom = PatchMarginBottom;
            styleBox.PatchMarginTop = PatchMarginTop;
            styleBox.PatchMarginRight = PatchMarginRight;
            styleBox.PatchMarginLeft = PatchMarginLeft;
        }
        else
        {
            styleBox.PatchMarginBottom = PatchMargin.Value.Bottom;
            styleBox.PatchMarginTop = PatchMargin.Value.Top;
            styleBox.PatchMarginRight = PatchMargin.Value.Right;
            styleBox.PatchMarginLeft = PatchMargin.Value.Left;
        }

        return styleBox;
    }
    
    private IRsiStateLike RsiStateLike(SpriteSpecifier specifier, IDependencyCollection dependencies)
    {
        var resC = dependencies.Resolve<IResourceCache>();
        switch (specifier)
        {
            case SpriteSpecifier.Texture tex:
                return tex.GetTexture(resC);
            case SpriteSpecifier.Rsi rsi:
                return rsi.GetState(resC);
            default:
                throw new NotSupportedException();
        }
    }
}