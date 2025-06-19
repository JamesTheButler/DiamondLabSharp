using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Rendering;
using Diamonds.Rendering.AxisScale;
using Diamonds.Utilities;

namespace Diamonds;

public partial class MainWindow
{
    private SizeSettings _currentSizeSettings = SizeSettings.Defaults;
    private ColorSettings _currentColorSettings = ColorSettings.Defaults;
    
    private const int InfoBarThickness = 50;
    
    private readonly Thickness _paintingMargin = new(10);
    private readonly Thickness _canvasMargin = new(10,10, 0, 0);
    
    public MainWindow()
    {
        InitializeComponent();
        SetUpUi();
        ReDraw();
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
        MainCanvas.Children.Clear();
        
        var paintingSizeSettings = _currentSizeSettings;
        var colorSettings = _currentColorSettings;

        var canvasSize = paintingSizeSettings.PaintingSize
            .Add(_canvasMargin)
            .Add(_paintingMargin)
            .Add(InfoBarThickness);
        
        MainCanvas.Width = canvasSize.Width;
        MainCanvas.Height = canvasSize.Height;
        MainCanvas.Background = new SolidColorBrush(MyColors.Background);

        var paintingOrigin = new Point(
            _canvasMargin.Left + InfoBarThickness + _paintingMargin.Left, 
            _canvasMargin.Top + InfoBarThickness + _paintingMargin.Top);

        DrawCanvasBackground(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawScales(paintingOrigin, paintingSizeSettings);
        DrawMountingRim(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawPaintingBackground(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawDiamondPattern(paintingOrigin, paintingSizeSettings, colorSettings);
    }

    private void DrawScales(Point paintingOrigin, SizeSettings sizeSettings)
    {
        GetDiamondTicks(sizeSettings, 
            out var horizontalTickPositions, 
            out var verticalTickPositions);

        var rim = sizeSettings.MountingRimSize;
        var totalMargin = rim + sizeSettings.CanvasMarginSize;
        
        var horizontalTicks = horizontalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]\n{p:0,#}")))
            .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
            .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
            .ToArray();

        var horizontalBar = new AxisScale(sizeSettings.PaintingSize.Width, Orientation.Horizontal)
        {
            Ticks = horizontalTicks,
        };
        Canvas.SetLeft(horizontalBar, paintingOrigin.X);
        Canvas.SetTop(horizontalBar, 0);
        MainCanvas.Children.Add(horizontalBar);
        
        var verticalTicks = verticalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]{p:0,#}")))
            .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]{p:0,#}")))
            .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - rim, new FormatterLabel(p => $"[M]{p:0,#}")))
            .ToArray();
        var verticalBar = new AxisScale(sizeSettings.PaintingSize.Height, Orientation.Vertical)
        {
            Ticks = verticalTicks,
        };
        
        Canvas.SetLeft(verticalBar, 0);
        Canvas.SetTop(verticalBar, paintingOrigin.Y);
        MainCanvas.Children.Add(verticalBar);
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
        var totals = _currentSizeSettings.PaintingSize;
        TotalWidth.Text = totals.Width.ToString(CultureInfo.CurrentCulture);
        TotalHeight.Text = totals.Height.ToString(CultureInfo.CurrentCulture);
        
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

    private void DrawCanvasBackground(Point origin, SizeSettings canvasSize, ColorSettings colors)
    {
        var canvasBackground = new Rectangle
        {
            Width = canvasSize.PaintingSize.Width,
            Height = canvasSize.PaintingSize.Height,
            Fill = new SolidColorBrush(colors.CanvasRimColor),
            Stroke = new SolidColorBrush(colors.MountingRimColor)
        };
        Canvas.SetLeft(canvasBackground, origin.X);
        Canvas.SetTop(canvasBackground, origin.Y);
        MainCanvas.Children.Add(canvasBackground);
    }

    private void DrawMountingRim(Point origin, SizeSettings size, ColorSettings colors)
    {
        var paintingSize = size.PaintingSize;
        var actualMountingRimColor = colors.MountingRimColor;
        actualMountingRimColor.A = 128;
        var mountingRim = new Polygon
        {
            Points =
            [
                new Point(0, 0),
                new Point(0, paintingSize.Height),
                new Point(paintingSize.Width, paintingSize.Height),
                new Point(paintingSize.Width, 0),
                new Point(size.MountingRimSize, 0),
                new Point(size.MountingRimSize, size.MountingRimSize),
                new Point(paintingSize.Width - size.MountingRimSize, size.MountingRimSize),
                new Point(paintingSize.Width - size.MountingRimSize, paintingSize.Height - size.MountingRimSize),
                new Point(size.MountingRimSize, paintingSize.Height - size.MountingRimSize),
                new Point(size.MountingRimSize, 0),
                new Point(0, 0),
                
            ],
            Fill = new SolidColorBrush(actualMountingRimColor),
        };
        Canvas.SetLeft(mountingRim, origin.X);
        Canvas.SetTop(mountingRim, origin.Y);
        MainCanvas.Children.Add(mountingRim);
        
        var mountingRimOutline = new Rectangle
        {
            Width = paintingSize.Width - 2 * size.MountingRimSize,
            Height = paintingSize.Height - 2 * size.MountingRimSize,
            Stroke = new SolidColorBrush(colors.MountingRimColor),
            StrokeThickness = 1,
        };
        Canvas.SetLeft(mountingRimOutline, origin.X + size.MountingRimSize);
        Canvas.SetTop(mountingRimOutline, origin.Y + size.MountingRimSize);
        MainCanvas.Children.Add(mountingRimOutline);
    }

    private void DrawPaintingBackground(Point origin, SizeSettings size, ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.CanvasMarginSize;
        var paintedBackground = new Rectangle
        {
            Width = size.GridColumns * size.DiamondWidth,
            Height = size.GridRows * size.DiamondHeight,
            Fill = new SolidColorBrush(colors.BackgroundColor)
        };
        Canvas.SetLeft(paintedBackground, origin.X + offset);
        Canvas.SetTop(paintedBackground, origin.Y + offset);
        MainCanvas.Children.Add(paintedBackground);
    }
    
    private void DrawDiamondPattern(Point origin, SizeSettings size,  ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.CanvasMarginSize;
        for (var row = 0; row < size.GridRows; row++)
        {
            for (var col = 0; col < size.GridColumns; col++)
            {
                var cx = origin.X  + offset + col * size.DiamondWidth + size.DiamondWidth / 2;
                var cy = origin.Y  + offset + row * size.DiamondHeight + size.DiamondHeight / 2;

                var diamond = new Polygon
                {
                    Points =
                    [
                        new Point(cx, cy - size.DiamondHeight / 2) ,
                        new Point(cx + size.DiamondWidth / 2, cy) ,
                        new Point(cx, cy + size.DiamondHeight / 2) ,
                        new Point(cx - size.DiamondWidth / 2, cy) 
                    ],
                    Fill = new SolidColorBrush(colors.DiamondColor),
                };
                MainCanvas.Children.Add(diamond);
            }
        }
    }

    private void GetDiamondTicks(SizeSettings size, out double[] horizontalTicks, out double[] verticalTicks)
    {
        verticalTicks = new double[size.GridRows];
        horizontalTicks = new double[size.GridColumns];
        
        var offset = size.MountingRimSize + size.CanvasMarginSize;
        for (var row = 0; row < size.GridRows; row++)
        {
            verticalTicks[row] = offset + size.DiamondHeight * .5 + size.DiamondHeight * row;
        }
        for (var col = 0; col < size.GridColumns; col++)
        {
            horizontalTicks[col] = offset + size.DiamondWidth * .5 + size.DiamondWidth * col;
        }
    }
    
    #region Event Handlers

    private void OnResetColorsButtonClicked(object sender, RoutedEventArgs e)
    {
        ResetColorInputs();
    }

    private void OnResetSizesButtonClicked(object sender, RoutedEventArgs e)
    {
        ResetSizeInputs();
    }

    private void OnPngButtonClicked(object sender, RoutedEventArgs e)
    {
        FileOperations.SaveCanvasToPng(MainCanvas, @"C:\Users\TomIlle\Desktop\diamonds.png");
    }

    private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Save TBD");
    }

    private void OnLoadButtonClicked(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Load TBD");
    }

    #endregion
}