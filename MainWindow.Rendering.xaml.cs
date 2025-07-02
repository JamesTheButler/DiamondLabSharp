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
    private void Render()
    {
        MainCanvas.Children.Clear();

        var sizeSettings = _model.SizeSettings;

        MainCanvas.Background = new SolidColorBrush(MyColors.Background);

        if (_model.DisplaySettings.ShowDebugLines)
        {
            var canvasOutline = new Rectangle
            {
                Width = MainCanvas.ActualWidth,
                Height = MainCanvas.ActualHeight,
                Stroke = new SolidColorBrush(MyColors.Debug)
            };
            canvasOutline.WithOrigin(0, 0);
            MainCanvas.Children.Add(canvasOutline);
        }

        var canvasCenter = new Point(MainCanvas.ActualWidth / 2f, MainCanvas.ActualHeight / 2f);

        var paintingOrigin = new Point(
            canvasCenter.X - sizeSettings.PaintingSize.Width / 2,
            canvasCenter.Y - sizeSettings.PaintingSize.Height / 2);

        RenderCanvasBackground(paintingOrigin);
        if (_model.DisplaySettings.ShowScales)
            RenderScales(paintingOrigin);

        _patternRenderer.Render(paintingOrigin);

        if (_model.DisplaySettings.ShowFrame)
            _frameRenderer.Render(paintingOrigin);
    }

    private void RenderCanvasBackground(Point origin)
    {
        var sizeSettings = _model.SizeSettings;
        var colorSettings = _model.ColorSettings;

        var canvasBackground = new Rectangle
        {
            Width = sizeSettings.PaintingSize.Width,
            Height = sizeSettings.PaintingSize.Height,
            Fill = new SolidColorBrush(colorSettings.CanvasRimColor),
            Stroke = new SolidColorBrush(colorSettings.MountingRimColor)
        };
        canvasBackground.WithOrigin(origin);
        MainCanvas.Children.Add(canvasBackground);
    }

    private void RenderScales(Point paintingOrigin)
    {
        var sizeSettings = _model.SizeSettings;
        var displaySettings = _model.DisplaySettings;

        var rim = sizeSettings.MountingRimSize;
        var totalMargin = rim + sizeSettings.PaintingMargin;

        var scaleTargetOrigin = displaySettings.OnlyPattern
            ? paintingOrigin + new Vector(totalMargin, totalMargin)
            : paintingOrigin;

        var scaleTargetSize = displaySettings.OnlyPattern
            ? sizeSettings.PatternSize
            : sizeSettings.PaintingSize;

        var horizontalTicks = _model.HorizontalDiamondTicks
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]\n{p:0,#}")));

        if (!displaySettings.OnlyPattern)
        {
            horizontalTicks = horizontalTicks
                .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
                .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
                .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - totalMargin,
                    new FormatterLabel(p => $"[R]\n{p:0,#}")))
                .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - rim,
                    new FormatterLabel(p => $"[M]\n{p:0,#}")));
        }

        var horizontalBar = new AxisScale(scaleTargetSize.Width, Orientation.Horizontal)
        {
            Ticks = horizontalTicks.ToArray()
        };

        var horizontalBarOrigin = scaleTargetOrigin with
        {
            Y = scaleTargetOrigin.Y - InfoBarThickness - _paintingMargin.Top
        };

        horizontalBar.WithOrigin(horizontalBarOrigin);

        MainCanvas.Children.Add(horizontalBar);

        var verticalTicks = _model.VerticalDiamondTicks
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]{p:0,#}")));

        if (!displaySettings.OnlyPattern)
        {
            verticalTicks = verticalTicks
                .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]{p:0,#}")))
                .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
                .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - totalMargin,
                    new FormatterLabel(p => $"[R]{p:0,#}")))
                .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - rim,
                    new FormatterLabel(p => $"[M]{p:0,#}")));
        }

        var verticalBar = new AxisScale(scaleTargetSize.Height, Orientation.Vertical)
        {
            Ticks = verticalTicks.ToArray()
        };

        var verticalBarOrigin = scaleTargetOrigin with
        {
            X = scaleTargetOrigin.X - InfoBarThickness - _paintingMargin.Left
        };

        MainCanvas.Children.Add(verticalBar.WithOrigin(verticalBarOrigin));


        if (!_model.DisplaySettings.ShowDebugLines)
            return;

        var horizontalOutline = new Rectangle
        {
            Width = scaleTargetSize.Width,
            Height = InfoBarThickness,
            Stroke = new SolidColorBrush(MyColors.Debug)
        };
        MainCanvas.Children.Add(horizontalOutline.WithOrigin(horizontalBarOrigin, ZIndex.Debug));

        var verticalOutline = new Rectangle
        {
            Width = InfoBarThickness,
            Height = scaleTargetSize.Height,
            Stroke = new SolidColorBrush(MyColors.Debug)
        };
        MainCanvas.Children.Add(verticalOutline.WithOrigin(verticalBarOrigin, ZIndex.Debug));
    }
}