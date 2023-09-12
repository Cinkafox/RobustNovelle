using System.Linq;
using Cinka.Game.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using static Robust.Client.UserInterface.StylesheetHelpers;

namespace Cinka.Game.StyleSheet;

public sealed class StyleNovelle : StyleBase
{
    public static readonly Color GoodGreenFore = Color.FromHex("#31843E");
    public static readonly Color ConcerningOrangeFore = Color.FromHex("#A5762F");
    public static readonly Color DangerousRedFore = Color.FromHex("#BB3232");
    public static readonly Color DisabledFore = Color.FromHex("#5A5A5A");

    public static readonly Color ButtonColorDefault = Color.FromHex("#464966");
    public static readonly Color ButtonColorDefaultRed = Color.FromHex("#D43B3B");
    public static readonly Color ButtonColorHovered = Color.FromHex("#575b7f");
    public static readonly Color ButtonColorHoveredRed = Color.FromHex("#DF6B6B");
    public static readonly Color ButtonColorPressed = Color.FromHex("#3e6c45");
    public static readonly Color ButtonColorDisabled = Color.FromHex("#30313c");

    public static readonly Color ButtonColorCautionDefault = Color.FromHex("#ab3232");
    public static readonly Color ButtonColorCautionHovered = Color.FromHex("#cf2f2f");
    public static readonly Color ButtonColorCautionPressed = Color.FromHex("#3e6c45");
    public static readonly Color ButtonColorCautionDisabled = Color.FromHex("#602a2a");

    public static readonly Color ButtonColorDialogDefault = Color.FromHex("#15273a");
    public static readonly Color ButtonColorDialogHovered = Color.FromHex("#15273a");
    public static readonly Color ButtonColorDialogPressed = Color.FromHex("#15273a");
    public static readonly Color ButtonColorDialogDisabled = Color.FromHex("#15273a");

    public static readonly Color ButtonColorGoodDefault = Color.FromHex("#3E6C45");
    public static readonly Color ButtonColorGoodHovered = Color.FromHex("#31843E");

    public static readonly Color PanelBackgroundDefault = Color.FromHex("#25252A");

    public StyleNovelle(IResourceCache resCache) : base(resCache)
    {
        var windowBackgroundTex = resCache.GetTexture("/Textures/Interface/Novelle/window_background.png");
        var windowBackground = new StyleBoxTexture
        {
            Texture = windowBackgroundTex
        };
        windowBackground.SetPatchMargin(StyleBox.Margin.Horizontal | StyleBox.Margin.Bottom, 2);
        windowBackground.SetExpandMargin(StyleBox.Margin.Horizontal | StyleBox.Margin.Bottom, 2);

        var borderedTransparentWindowBackgroundTex =
            resCache.GetTexture("/Textures/Interface/Novelle/transparent_window_background_bordered.png");
        var borderedTransparentWindowBackground = new StyleBoxTexture
        {
            Texture = borderedTransparentWindowBackgroundTex
        };
        borderedTransparentWindowBackground.SetPatchMargin(StyleBox.Margin.All, 2);


        Stylesheet = new Stylesheet(BaseRules.Concat(new[]
        {
            new StyleRule(
                new SelectorElement(null, new[] { DefaultWindow.StyleClassWindowPanel }, null, null),
                new[]
                {
                    new StyleProperty(PanelContainer.StylePropertyPanel, windowBackground)
                }),

            Element<PanelContainer>().Class(ClassAngleRect)
                .Prop(PanelContainer.StylePropertyPanel, BaseAngleRect)
                .Prop(Control.StylePropertyModulateSelf, PanelBackgroundDefault),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                .Prop(ContainerButton.StylePropertyStyleBox, BaseButton),

            new StyleRule(new SelectorElement(typeof(Label), new[] { ContainerButton.StyleClassButton }, null, null),
                new[]
                {
                    new StyleProperty(Label.StylePropertyAlignMode, Label.AlignMode.Center)
                }),

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

            // Colors for the meow buttons.
            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonDialog)
                .Pseudo(ContainerButton.StylePseudoClassNormal)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorDialogDefault),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonDialog)
                .Pseudo(ContainerButton.StylePseudoClassHover)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorDialogHovered),

            Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonDialog)
                .Pseudo(ContainerButton.StylePseudoClassPressed)
                .Prop(Control.StylePropertyModulateSelf, ButtonColorDialogPressed)
        }).ToList());
    }

    public override Stylesheet Stylesheet { get; }
}