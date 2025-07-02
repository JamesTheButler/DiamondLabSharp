using System.Windows;
using System.Windows.Controls;

namespace Diamonds.Utilities;

public static class UiElementExtension
{
    public static T WithOrigin<T>(this T uiElement, double x, double y, int zIndex = 0) where T : UIElement
    {
        return uiElement.WithOrigin(new Point(x, y), zIndex);
    }

    public static T WithOrigin<T>(this T uiElement, Point origin, int zIndex = 0) where T : UIElement
    {
        Canvas.SetLeft(uiElement, origin.X);
        Canvas.SetTop(uiElement, origin.Y);
        Panel.SetZIndex(uiElement, zIndex);

        return uiElement;
    }
}