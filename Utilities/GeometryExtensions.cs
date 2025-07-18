using System.Windows;

namespace DiamondLab.Utilities;

public static class GeometryExtensions
{
    public static Size Add(this Size size, Size size2)
    {
        return new Size(size.Width + size2.Width, size.Height + size2.Height);
    }

    public static Size Add(this Size size, Thickness margin)
    {
        return new Size(size.Width + margin.Left + margin.Right, size.Height + margin.Top + margin.Bottom);
    }

    public static Size Add(this Size size, double addend)
    {
        return new Size(size.Width + addend, size.Height + addend);
    }

    public static Size Add(this Size size, int addend)
    {
        return new Size(size.Width + addend, size.Height + addend);
    }

    public static Vector AsOffset(this Size size)
    {
        return new Vector(size.Width, size.Height);
    }

    public static Point Add(this Point point, int addend)
    {
        return new Point(point.X + addend, point.Y + addend);
    }
}