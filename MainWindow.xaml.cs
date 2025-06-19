using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diamonds;

public partial class MainWindow
{
    private SizeSettings _currentSizeSettings = SizeSettings.Defaults;
    private ColorSettings _currentColorSettings = ColorSettings.Defaults;
    
    public MainWindow()
    {
        InitializeComponent();
        SetUpUi();
        ReDraw();
    }

    private void ApplyColors()
    {
        _currentColorSettings = new ColorSettings(
            BackgroundColorInput.SelectedColor ?? ColorSettings.Defaults.BackgroundColor,
            DiamondColorInput.SelectedColor ?? ColorSettings.Defaults.DiamondColor,
            CanvasRimColorInput.SelectedColor ?? ColorSettings.Defaults.CanvasRimColor,
            MountingRimColorInput.SelectedColor ?? ColorSettings.Defaults.MountingRimColor);
        
        ReDraw();
    }
    
    private void ApplySizes()
    {
        _currentSizeSettings = new SizeSettings(
            DiamondWidthInput.Text.ToDouble() ?? SizeSettings.Defaults.DiamondWidth,
            DiamondHeightInput.Text.ToDouble() ?? SizeSettings.Defaults.DiamondHeight,
            ColumnsInput.Value ?? SizeSettings.Defaults.GridColumns,
            RowsInput.Value ?? SizeSettings.Defaults.GridRows,
            OuterRimSizeInput.Text.ToDouble() ?? SizeSettings.Defaults.CanvasMarginSize,
            MountingRimSizeInput.Text.ToDouble() ?? SizeSettings.Defaults.MountingRimSize);
        
        ReDraw();
    }

    private void ResetColorInputs()
    {
        BackgroundColorInput.SelectedColor = ColorSettings.Defaults.BackgroundColor;
        MountingRimColorInput.SelectedColor = ColorSettings.Defaults.MountingRimColor;
        DiamondColorInput.SelectedColor = ColorSettings.Defaults.DiamondColor;
        CanvasRimColorInput.SelectedColor = ColorSettings.Defaults.CanvasRimColor;
    }
    private void ResetSizeInputs()
    {
        DiamondWidthInput.Text = SizeSettings.Defaults.DiamondWidth.ToString(CultureInfo.CurrentCulture);
        DiamondHeightInput.Text = SizeSettings.Defaults.DiamondHeight.ToString(CultureInfo.CurrentCulture);
        ColumnsInput.Text = SizeSettings.Defaults.GridColumns.ToString();
        RowsInput.Text = SizeSettings.Defaults.GridRows.ToString();
        OuterRimSizeInput.Text = SizeSettings.Defaults.CanvasMarginSize.ToString(CultureInfo.CurrentCulture);
        MountingRimSizeInput.Text = SizeSettings.Defaults.MountingRimSize.ToString(CultureInfo.CurrentCulture);
    }
    
    private void SetUpUi()
    {
        ResetColorInputs();
        ResetSizeInputs();
        
        BackgroundColorInput.SelectedColorChanged += (_, _) => ApplyColors();
        MountingRimColorInput.SelectedColorChanged += (_, _) => ApplyColors();
        DiamondColorInput.SelectedColorChanged += (_, _) => ApplyColors();
        CanvasRimColorInput.SelectedColorChanged += (_, _) => ApplyColors();
        
        DiamondWidthInput.TextChanged += (_, _) => ApplySizes();
        DiamondHeightInput.TextChanged += (_, _) => ApplySizes();
        ColumnsInput.ValueChanged += (_, _) => ApplySizes();
        RowsInput.ValueChanged += (_, _) => ApplySizes();
        OuterRimSizeInput.TextChanged += (_, _) => ApplySizes();
        MountingRimSizeInput.TextChanged += (_, _) => ApplySizes();
    }

    private void ReDraw()
    {
        var size = _currentSizeSettings;
        var colors = _currentColorSettings;

        var canvasSize = size.GetCanvasSize();
        MyCanvas.Width = canvasSize.Width;
        MyCanvas.Height = canvasSize.Height;
        MyCanvas.Children.Clear();
        MyCanvas.Background = new SolidColorBrush(colors.CanvasRimColor);
        
        DrawCanvasBackground(canvasSize, colors);
        DrawMountingRim(canvasSize, size, colors);
        DrawPaintedBackground(size, colors);
        DrawDiamondPattern(size, colors);
    }

    private void DrawCanvasBackground(Size canvasSize, ColorSettings colors)
    {
        var canvasBackground = new Rectangle
        {
            Width = canvasSize.Width,
            Height = canvasSize.Height,
            Fill = new SolidColorBrush(colors.CanvasRimColor),
            Stroke = new SolidColorBrush(colors.MountingRimColor)
        };
        MyCanvas.Children.Add(canvasBackground);
    }

    private void DrawMountingRim(Size canvasSize, SizeSettings size, ColorSettings colors)
    {
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
        
        var mountingRimOutline = new Rectangle
        {
            Width = canvasSize.Width - 2 * size.MountingRimSize,
            Height = canvasSize.Height - 2 * size.MountingRimSize,
            Stroke = new SolidColorBrush(colors.MountingRimColor),
            StrokeThickness = 1,
        };
        Canvas.SetLeft(mountingRimOutline, size.MountingRimSize);
        Canvas.SetTop(mountingRimOutline, size.MountingRimSize);
        MyCanvas.Children.Add(mountingRimOutline);
    }

    private void DrawDiamondPattern(SizeSettings size,  ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.CanvasMarginSize;
        for (var row = 0; row < size.GridRows; row++)
        {
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

    private void DrawPaintedBackground(SizeSettings size, ColorSettings colors)
    {
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
    }

    private void OnResetColorsButtonClicked(object sender, RoutedEventArgs e)
    {
        ResetColorInputs();
    }

    private void OnResetSizesButtonClicked(object sender, RoutedEventArgs e)
    {
        ResetSizeInputs();
    }
}