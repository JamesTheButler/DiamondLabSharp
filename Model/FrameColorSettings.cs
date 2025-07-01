using System.Windows.Media;
using Diamonds.Rendering;

namespace Diamonds.Model;

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