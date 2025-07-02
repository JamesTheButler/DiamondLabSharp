using System.Windows.Media;

namespace Diamonds.Utilities;

public static class ColorExtensions
{
    public static Color Darken(this Color color, double factor = 0.8)
    {
        return Color.FromArgb(
            color.A,
            (byte)(color.R * factor),
            (byte)(color.G * factor),
            (byte)(color.B * factor)
        );
    }
}