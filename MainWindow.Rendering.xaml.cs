using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Model;
using Diamonds.Rendering;
using Diamonds.Rendering.AxisScale;
using Diamonds.Utilities;
using Xceed.Wpf.Toolkit;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

namespace Diamonds;

public partial class MainWindow
{
    private void ReDraw()
    {
        MainCanvas.Children.Clear();

        var canvasSize = _sizeSettings.PaintingSize
            .Add(_canvasMargin)
            .Add(_paintingMargin)
            .Add(InfoBarThickness);

        MainCanvas.Width = canvasSize.Width;
        MainCanvas.Height = canvasSize.Height;
        MainCanvas.Background = new SolidColorBrush(MyColors.Background);

        var paintingOrigin = new Point(
            _canvasMargin.Left + InfoBarThickness + _paintingMargin.Left,
            _canvasMargin.Top + InfoBarThickness + _paintingMargin.Top);

        DrawCanvasBackground(paintingOrigin);
        if (_displaySettings.ShowScales)
            DrawScales(paintingOrigin);

        DrawMountingRim(paintingOrigin);
        DrawPaintingBackground(paintingOrigin);
        DrawDiamondPattern(paintingOrigin);
    }

    private void DrawCanvasBackground(Point origin)
    {
        var canvasBackground = new Rectangle
        {
            Width = _sizeSettings.PaintingSize.Width,
            Height = _sizeSettings.PaintingSize.Height,
            Fill = new SolidColorBrush(_colorSettings.CanvasRimColor),
            Stroke = new SolidColorBrush(_colorSettings.MountingRimColor)
        };
        Canvas.SetLeft(canvasBackground, origin.X);
        Canvas.SetTop(canvasBackground, origin.Y);
        MainCanvas.Children.Add(canvasBackground);
    }

    private void DrawScales(Point paintingOrigin)
    {
        GetDiamondTicks(_sizeSettings,
            out var horizontalTickPositions,
            out var verticalTickPositions);

        var rim = _sizeSettings.MountingRimSize;
        var totalMargin = rim + _sizeSettings.PaintingMargin;

        var scaleTargetOrigin = _displaySettings.OnlyPattern
            ? paintingOrigin + new Vector(totalMargin, totalMargin)
            : paintingOrigin;

        var scaleTargetSize = _displaySettings.OnlyPattern
            ? _sizeSettings.PatternSize
            : _sizeSettings.PaintingSize;

        var horizontalTicks = horizontalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]\n{p:0,#}")));

        if (!_displaySettings.OnlyPattern)
        {
            horizontalTicks = horizontalTicks
                .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
                .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
                .Append(new AxisScaleTick(_sizeSettings.PaintingSize.Width - totalMargin,
                    new FormatterLabel(p => $"[R]\n{p:0,#}")))
                .Append(new AxisScaleTick(_sizeSettings.PaintingSize.Width - rim,
                    new FormatterLabel(p => $"[M]\n{p:0,#}")));
        }


        var horizontalBar = new AxisScale(scaleTargetSize.Width, Orientation.Horizontal)
        {
            Ticks = horizontalTicks.ToArray()
        };

        Canvas.SetLeft(horizontalBar, scaleTargetOrigin.X);
        Canvas.SetTop(horizontalBar, 0);

        MainCanvas.Children.Add(horizontalBar);

        var verticalTicks = verticalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]{p:0,#}")));

        if (!_displaySettings.OnlyPattern)
        {
            verticalTicks = verticalTicks
                .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]{p:0,#}")))
                .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
                .Append(new AxisScaleTick(_sizeSettings.PaintingSize.Height - totalMargin,
                    new FormatterLabel(p => $"[R]{p:0,#}")))
                .Append(new AxisScaleTick(_sizeSettings.PaintingSize.Height - rim,
                    new FormatterLabel(p => $"[M]{p:0,#}")));
        }

        var verticalBar = new AxisScale(scaleTargetSize.Height, Orientation.Vertical)
        {
            Ticks = verticalTicks.ToArray()
        };


        Canvas.SetLeft(verticalBar, 0);
        Canvas.SetTop(verticalBar, scaleTargetOrigin.Y);

        MainCanvas.Children.Add(verticalBar);
    }

    private void DrawMountingRim(Point origin)
    {
        var paintingSize = _sizeSettings.PaintingSize;
        var actualMountingRimColor = _colorSettings.MountingRimColor;
        actualMountingRimColor.A = 128;

        var rim = _sizeSettings.MountingRimSize;
        var mountingRim = new Polygon
        {
            Points =
            [
                new Point(0, 0),
                new Point(0, paintingSize.Height),
                new Point(paintingSize.Width, paintingSize.Height),
                new Point(paintingSize.Width, 0),
                new Point(rim, 0),
                new Point(rim, rim),
                new Point(paintingSize.Width - rim, rim),
                new Point(paintingSize.Width - rim, paintingSize.Height - rim),
                new Point(rim, paintingSize.Height - rim),
                new Point(rim, 0),
                new Point(0, 0)
            ],
            Fill = new SolidColorBrush(actualMountingRimColor)
        };
        Canvas.SetLeft(mountingRim, origin.X);
        Canvas.SetTop(mountingRim, origin.Y);
        MainCanvas.Children.Add(mountingRim);

        var mountingRimOutline = new Rectangle
        {
            Width = paintingSize.Width - 2 * rim,
            Height = paintingSize.Height - 2 * rim,
            Stroke = new SolidColorBrush(_colorSettings.MountingRimColor),
            StrokeThickness = 1
        };
        Canvas.SetLeft(mountingRimOutline, origin.X + rim);
        Canvas.SetTop(mountingRimOutline, origin.Y + rim);
        MainCanvas.Children.Add(mountingRimOutline);
    }

    private void DrawPaintingBackground(Point origin)
    {
        var offset = _sizeSettings.MountingRimSize + _sizeSettings.PaintingMargin;
        var paintedBackground = new Rectangle
        {
            Width = _sizeSettings.GridColumns * _sizeSettings.DiamondWidth,
            Height = _sizeSettings.GridRows * _sizeSettings.DiamondHeight,
            Fill = new SolidColorBrush(_colorSettings.BackgroundColor)
        };
        Canvas.SetLeft(paintedBackground, origin.X + offset);
        Canvas.SetTop(paintedBackground, origin.Y + offset);
        MainCanvas.Children.Add(paintedBackground);
    }

    private void DrawDiamondPattern(Point paintingOrigin)
    {
        var offset = _sizeSettings.MountingRimSize + _sizeSettings.PaintingMargin;
        var patternOrigin = new Point(paintingOrigin.X + offset, paintingOrigin.Y + offset);

        var highlights =
            _highlightSettings.Highlights
                .GroupBy(highlight => highlight.Position)
                .ToDictionary(group => group.Key, g => g.Last().Color);

        for (var row = -1; row <= _sizeSettings.GridRows; row++)
        {
            for (var col = -1; col <= _sizeSettings.GridColumns; col++)
            {
                if (!highlights.TryGetValue(new Point(row, col), out var color))
                    color = _colorSettings.DiamondColor;

                var centerX =
                    patternOrigin.X +
                    _sizeSettings.OffsetX +
                    col * _sizeSettings.DiamondWidth +
                    _sizeSettings.DiamondWidth * .5;

                var centerY =
                    patternOrigin.Y +
                    _sizeSettings.OffsetY +
                    row * _sizeSettings.DiamondHeight +
                    _sizeSettings.DiamondHeight * .5;

                var diamond = new Diamond(new Point(centerX, centerY), _sizeSettings.DiamondSize, color)
                {
                    Clip = new RectangleGeometry(new Rect(patternOrigin, _sizeSettings.PatternSize))
                };

                var row1 = row;
                var col1 = col;
                diamond.Clicked += () => { OpenColorPicker(row1, col1, color); };
                diamond.RightClicked += () =>
                {
                    RemoveHighlight(row1, col1);
                    ReDraw();
                };

                MainCanvas.Children.Add(diamond.Shape);
            }
        }
    }

    private void RemoveHighlight(int row, int col)
    {
        _highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == new Point(row, col));
    }

    private void GetDiamondTicks(SizeSettings size, out double[] horizontalTicks, out double[] verticalTicks)
    {
        verticalTicks = new double[size.GridRows];
        horizontalTicks = new double[size.GridColumns];

        var offset = _displaySettings.OnlyPattern
            ? (Point)size.Offset
            : new Point(
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

    private void OpenColorPicker(int highlightRow, int highlightCol, Color initialColor)
    {
        var picker = new ColorPicker
        {
            SelectedColor = initialColor,
            Width = 200,
            Height = 30
        };

        var button = new Button
        {
            Content = "Select Highlight Color"
        };
        var dialogPanel = new StackPanel();
        dialogPanel.Children.Add(picker);
        dialogPanel.Children.Add(button);

        var dialog = new Window
        {
            Title = "Select Highlight Color",
            Content = dialogPanel,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this,
            ResizeMode = ResizeMode.NoResize
        };

        picker.SelectedColorChanged += (_, _) =>
        {
            var position = new Point(highlightRow, highlightCol);
            _highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == position);
            var newHighlight = new Highlight(position, picker.SelectedColor ?? initialColor, false);
            _highlightSettings.Highlights.Add(newHighlight);
            ReDraw();
        };

        button.Click += (_, _) => { dialog.Close(); };

        dialog.Show();
    }
}