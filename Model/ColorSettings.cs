using System.Windows.Media;
using DiamondLab.Rendering;

namespace DiamondLab.Model;

public readonly record struct ColorSettings(
    Color BackgroundColor,
    Color DiamondColor,
    Color CanvasRimColor,
    Color MountingRimColor)
{
    public static ColorSettings Defaults => new(
        MyColors.Light,
        MyColors.Dark,
        Colors.White,
        MyColors.Darkest
    );
}