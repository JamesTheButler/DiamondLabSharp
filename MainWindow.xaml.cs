using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diamonds;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DrawPattern();
    }

    private void DrawPattern()
    {
        var size = SizeSettings.Defaults;
        var colors = ColorSettings.Defaults;

        var canvasSize = size.GetCanvasSize();
        MyCanvas.Width = canvasSize.Width;
        MyCanvas.Height = canvasSize.Height;
        MyCanvas.Background = new SolidColorBrush(colors.CanvasColor);
        
        // canvas background
        var canvasBackground = new Rectangle
        {
            Width = canvasSize.Width,
            Height = canvasSize.Height,
            Fill = new SolidColorBrush(colors.CanvasColor),
            Stroke = new SolidColorBrush(colors.DiamondColor)
        };
        MyCanvas.Children.Add(canvasBackground);
        
        // mounting rim
        var actualMountingRimColor = colors.MountingRimColor;
        actualMountingRimColor.A = 128;
        var mountingRim = new Polygon
        {
            Points =
            [
                new Point(0, 0),
                new Point(0, canvasSize.Height),
                new Point(canvasSize.Width, canvasSize.Height),
                new Point(canvasSize.Width, 0),
                new Point(size.MountingRimSize, 0),
                new Point(size.MountingRimSize, size.MountingRimSize),
                new Point(canvasSize.Width - size.MountingRimSize, size.MountingRimSize),
                new Point(canvasSize.Width - size.MountingRimSize, canvasSize.Height - size.MountingRimSize),
                new Point(size.MountingRimSize, canvasSize.Height - size.MountingRimSize),
                new Point(size.MountingRimSize, 0),
                new Point(0, 0),
                
            ],
            Fill = new SolidColorBrush(actualMountingRimColor),
        };
        MyCanvas.Children.Add(mountingRim);
        
        // background of painted area
        var offset = size.MountingRimSize + size.CanvasMarginSize;
        var paintedBackground = new Rectangle
        {
            Width = size.GridColumns * size.DiamondWidth,
            Height = size.GridRows * size.DiamondHeight,
            Fill = new SolidColorBrush(colors.BackgroundColor)
        };
        Canvas.SetLeft(paintedBackground, offset);
        Canvas.SetTop(paintedBackground, offset);
        MyCanvas.Children.Add(paintedBackground);

        for (var row = 0; row < size.GridRows; row++)
        for (var col = 0; col < size.GridColumns; col++)
        {
            var cx = offset + col * size.DiamondWidth + size.DiamondWidth / 2;
            var cy = offset + row * size.DiamondHeight + size.DiamondHeight / 2;

            var diamond = new Polygon
            {
                Points =
                [
                    new Point(cx, cy - size.DiamondHeight / 2),
                    new Point(cx + size.DiamondWidth / 2, cy),
                    new Point(cx, cy + size.DiamondHeight / 2),
                    new Point(cx - size.DiamondWidth / 2, cy)
                ],
                Fill = new SolidColorBrush(colors.DiamondColor),
            };
            MyCanvas.Children.Add(diamond);
        }
    }
}