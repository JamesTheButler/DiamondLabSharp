using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Model;
using Diamonds.Rendering;
using Diamonds.Rendering.AxisScale;
using Diamonds.Utilities;

namespace Diamonds;

public partial class MainWindow
{
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
        if(_currentDisplaySettings.ShowScales)
        {
            DrawScales(paintingOrigin, paintingSizeSettings);
        }
        DrawMountingRim(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawPaintingBackground(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawDiamondPattern(paintingOrigin, paintingSizeSettings, colorSettings);
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

    private void DrawScales(Point paintingOrigin, SizeSettings sizeSettings)
    {
        GetDiamondTicks(sizeSettings,
            out var horizontalTickPositions,
            out var verticalTickPositions);

        var rim = sizeSettings.MountingRimSize;
        var totalMargin = rim + sizeSettings.PaintingMargin;

        var horizontalTicks = horizontalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]\n{p:0,#}")))
            .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
            .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - totalMargin,
                new FormatterLabel(p => $"[R]\n{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
            .ToArray();

        var horizontalBar = new AxisScale(sizeSettings.PaintingSize.Width, Orientation.Horizontal)
        {
            Ticks = horizontalTicks
        };
        Canvas.SetLeft(horizontalBar, paintingOrigin.X);
        Canvas.SetTop(horizontalBar, 0);
        MainCanvas.Children.Add(horizontalBar);

        var verticalTicks = verticalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]{p:0,#}")))
            .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]{p:0,#}")))
            .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - totalMargin,
                new FormatterLabel(p => $"[R]{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - rim, new FormatterLabel(p => $"[M]{p:0,#}")))
            .ToArray();
        var verticalBar = new AxisScale(sizeSettings.PaintingSize.Height, Orientation.Vertical)
        {
            Ticks = verticalTicks
        };

        Canvas.SetLeft(verticalBar, 0);
        Canvas.SetTop(verticalBar, paintingOrigin.Y);
        MainCanvas.Children.Add(verticalBar);
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
                new Point(0, 0)
            ],
            Fill = new SolidColorBrush(actualMountingRimColor)
        };
        Canvas.SetLeft(mountingRim, origin.X);
        Canvas.SetTop(mountingRim, origin.Y);
        MainCanvas.Children.Add(mountingRim);

        var mountingRimOutline = new Rectangle
        {
            Width = paintingSize.Width - 2 * size.MountingRimSize,
            Height = paintingSize.Height - 2 * size.MountingRimSize,
            Stroke = new SolidColorBrush(colors.MountingRimColor),
            StrokeThickness = 1
        };
        Canvas.SetLeft(mountingRimOutline, origin.X + size.MountingRimSize);
        Canvas.SetTop(mountingRimOutline, origin.Y + size.MountingRimSize);
        MainCanvas.Children.Add(mountingRimOutline);
    }

    private void DrawPaintingBackground(Point origin, SizeSettings size, ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.PaintingMargin;
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

    private void DrawDiamondPattern(Point paintingOrigin, SizeSettings size, ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.PaintingMargin;
        var patternOrigin = new Point(paintingOrigin.X + offset, paintingOrigin.Y + offset);
        
        var highlights = 
            _highlightSettings.Highlights
                .GroupBy(highlight => highlight.Position)
                .ToDictionary(group => group.Key, g => g.Last().Color);
        
        for (var row = -1; row <= size.GridRows; row++)
        {
            for (var col = -1; col <= size.GridColumns; col++)
            {
                if (!highlights.TryGetValue(new Point(row, col), out var color))
                {
                    color = colors.DiamondColor;
                }
                
                var cx = patternOrigin.X + size.OffsetX + col * size.DiamondWidth + size.DiamondWidth * .5;
                var cy = patternOrigin.Y + size.OffsetY + row * size.DiamondHeight + size.DiamondHeight * .5;
                var diamond = new Diamond(new Point(cx, cy), size.DiamondSize, color)
                { 
                    Clip = new RectangleGeometry(new Rect(patternOrigin, size.PatternSize))
                };
                var row1 = row;
                var col1 = col;
                diamond.Clicked += () => { AddHighlight(row1, col1); ReDraw(); };
                diamond.WheelClicked += () => { Console.WriteLine("open color picker"); };
                diamond.RightClicked += () => { RemoveHighlight(row1, col1); ReDraw(); };

                MainCanvas.Children.Add(diamond.Shape);
            }
        }
    }

    private void AddHighlight(int row, int col)
    {
        _highlightSettings.Highlights.Add(new Highlight(new(row, col), MyColors.Lightest, false));
    }

    private void RemoveHighlight(int row, int col)
    {
        _highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == new Point(row, col));
    }

    
    private void GetDiamondTicks(SizeSettings size, out double[] horizontalTicks, out double[] verticalTicks)
    {
        verticalTicks = new double[size.GridRows];
        horizontalTicks = new double[size.GridColumns];
        
        var offset = new Point(
            size.MountingRimSize + size.PaintingMargin + size.OffsetX,
            size.MountingRimSize + size.PaintingMargin + size.OffsetY);
        
        for (var col = 0; col < size.GridColumns; col++)
        {
            horizontalTicks[col] = offset.X + size.DiamondWidth * .5 + size.DiamondWidth * col;
        }

        for (var row = 0; row < size.GridRows; row++)
        {
            verticalTicks[row] = offset.Y + size.DiamondHeight * .5 + size.DiamondHeight * row;
        }

    }
}