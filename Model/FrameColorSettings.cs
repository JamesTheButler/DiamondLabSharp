using System.Windows.Media;
using DiamondLab.Rendering;

namespace DiamondLab.Model;

public readonly record struct FrameColorSettings(
    Color StructuralLayerColor,
    Color DecorativeLayer1Color,
    Color DecorativeLayer2Color)
{
    public static FrameColorSettings Defaults => new(
        MyColors.Darkest,
        MyColors.Dark,
        MyColors.Dark
    );
}