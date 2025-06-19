using System.Windows.Media;

namespace Diamonds;

public readonly record struct ColorSettings(
    Color BackgroundColor,
    Color DiamondColor,
    Color CanvasColor,
    Color MountingRimColor)
{
    public static ColorSettings Defaults => new(
        BackgroundColor: MyColors.Light,
        DiamondColor: MyColors.Dark,
        CanvasColor: Colors.White, 
        MountingRimColor: MyColors.Darkest
    );
}