using System.Linq;
using Content.Game.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using static Robust.Client.UserInterface.StylesheetHelpers;

namespace Content.Game.StyleSheet;

public sealed class StyleNovelle : StyleBase
{
    public static readonly Color GoodGreenFore = Color.FromHex("#31843E");
    public static readonly Color ConcerningOrangeFore = Color.FromHex("#A5762F");
    public static readonly Color DangerousRedFore = Color.FromHex("#BB3232");
    public static readonly Color DisabledFore = Color.FromHex("#5A5A5A");
    
    public static readonly Color ButtonTextColor = Color.Black;
    public static readonly Color TextColor = Color.Black;

    public static readonly Color ButtonColorDefault = Color.FromHex("#ffffff");
    public static readonly Color ButtonColorHovered = Color.FromHex("#575b7f");
    public static readonly Color ButtonColorPressed = Color.FromHex("#3e6c45");
    public static readonly Color ButtonColorDisabled = Color.FromHex("#30313c");

    public static readonly Color ButtonColorCautionDefault = Color.FromHex("#ab3232");
    public static readonly Color ButtonColorCautionHovered = Color.FromHex("#cf2f2f");
    public static readonly Color ButtonColorCautionPressed = Color.FromHex("#3e6c45");
    public static readonly Color ButtonColorCautionDisabled = Color.FromHex("#602a2a");

    public static readonly Color PanelBackgroundDefault = Color.FromHex("#25252A");

    public static readonly string BackgroundPath = "/Textures/Interface/Novelle/paper_background_book.svg.96dpi.png";
    public static readonly Box2 BackgroundPatchMargin = new Box2(23.0f, 16.0f, 14.0f, 15.0f);

    public StyleNovelle(IResourceCache resCache) : base(resCache)
    {
        var backgroundImage = IoCManager.Resolve<IResourceCache>().GetResource<TextureResource>(BackgroundPath);

        var backgroundImageMode = StyleBoxTexture.StretchMode.Stretch;
        var backgroundPatchMargin = BackgroundPatchMargin;
        var sb = new StyleBoxTexture
        {
            Texture = backgroundImage,
            Mode = backgroundImageMode,
            PatchMarginLeft = backgroundPatchMargin.Left,
            PatchMarginBottom = backgroundPatchMargin.Bottom,
            PatchMarginRight = backgroundPatchMargin.Right,
            PatchMarginTop = backgroundPatchMargin.Top,
        };
        
        var backgroundButtonImage = IoCManager.Resolve<IResourceCache>().GetResource<TextureResource>(BackgroundPath);

        var backgroundButtonImageMode = StyleBoxTexture.StretchMode.Stretch;
        var backgroundButtonPatchMargin = BackgroundPatchMargin;
        var sbb = new StyleBoxTexture
        {
            Texture = backgroundButtonImage,
            Mode = backgroundButtonImageMode,
            PatchMarginLeft = backgroundButtonPatchMargin.Left,
            PatchMarginBottom = backgroundButtonPatchMargin.Bottom,
            PatchMarginRight = backgroundButtonPatchMargin.Right,
            PatchMarginTop = backgroundButtonPatchMargin.Top,
        };


        Stylesheet = new Stylesheet(BaseRules.Concat(new[]
        {
            (StyleRule) Element<PanelContainer>().Class("DialogWindow")
                .Prop(PanelContainer.StylePropertyPanel, sb),
            
            Element<PanelContainer>().Class(ClassAngleRect)
                .Prop(PanelContainer.StylePropertyPanel, BaseAngleRect)
                .Prop(Control.StylePropertyModulateSelf, PanelBackgroundDefault),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Prop(ContainerButton.StylePropertyStyleBox, BaseButton),
            
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Prop(Label.StylePropertyAlignMode,Label.AlignMode.Center)
                .Prop(ContainerButton.StylePropertyStyleBox,sbb).Prop(Label.StylePropertyFontColor,ButtonTextColor),

            // Colors for the buttons.
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Pseudo(ContainerButton.StylePseudoClassNormal)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorDefault),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Pseudo(ContainerButton.StylePseudoClassHover)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorHovered),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Pseudo(ContainerButton.StylePseudoClassPressed)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorPressed),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Pseudo(ContainerButton.StylePseudoClassDisabled)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorDisabled),

            // Colors for the caution buttons.
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                .Pseudo(ContainerButton.StylePseudoClassNormal)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionDefault),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                .Pseudo(ContainerButton.StylePseudoClassHover)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionHovered),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                .Pseudo(ContainerButton.StylePseudoClassPressed)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionPressed),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                .Pseudo(ContainerButton.StylePseudoClassDisabled)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionDisabled),
        }).ToList());
    }

    public override Stylesheet Stylesheet { get; }
}