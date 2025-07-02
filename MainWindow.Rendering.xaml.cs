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

        RenderMountingRim(paintingOrigin);
        RenderPaintingBackground(paintingOrigin);
        RenderDiamondPattern(paintingOrigin);

        if (_model.DisplaySettings.ShowFrame)
            RenderFrame(paintingOrigin);
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

        GetDiamondTicks(sizeSettings,
            out var horizontalTickPositions,
            out var verticalTickPositions);

        var rim = sizeSettings.MountingRimSize;
        var totalMargin = rim + sizeSettings.PaintingMargin;

        var scaleTargetOrigin = displaySettings.OnlyPattern
            ? paintingOrigin + new Vector(totalMargin, totalMargin)
            : paintingOrigin;

        var scaleTargetSize = displaySettings.OnlyPattern
            ? sizeSettings.PatternSize
            : sizeSettings.PaintingSize;

        var horizontalTicks = horizontalTickPositions
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

        var verticalTicks = verticalTickPositions
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

    private void RenderMountingRim(Point origin)
    {
        var sizeSettings = _model.SizeSettings;
        var colorSettings = _model.ColorSettings;

        var paintingSize = sizeSettings.PaintingSize;
        var actualMountingRimColor = colorSettings.MountingRimColor;
        actualMountingRimColor.A = 128;

        var rim = sizeSettings.MountingRimSize;
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

        MainCanvas.Children.Add(mountingRim.WithOrigin(origin));

        var mountingRimOutline = new Rectangle
        {
            Width = paintingSize.Width - 2 * rim,
            Height = paintingSize.Height - 2 * rim,
            Stroke = new SolidColorBrush(colorSettings.MountingRimColor),
            StrokeThickness = 1
        };
        MainCanvas.Children.Add(mountingRimOutline.WithOrigin(origin.Add(rim)));
    }

    private void RenderPaintingBackground(Point origin)
    {
        var sizeSettings = _model.SizeSettings;
        var colorSettings = _model.ColorSettings;

        var offset = sizeSettings.MountingRimSize + sizeSettings.PaintingMargin;
        var paintedBackground = new Rectangle
        {
            Width = sizeSettings.GridColumns * sizeSettings.DiamondWidth,
            Height = sizeSettings.GridRows * sizeSettings.DiamondHeight,
            Fill = new SolidColorBrush(colorSettings.BackgroundColor)
        };
        MainCanvas.Children.Add(paintedBackground.WithOrigin(origin.Add(offset)));
    }

    private void RenderDiamondPattern(Point paintingOrigin)
    {
        var sizeSettings = _model.SizeSettings;
        var colorSettings = _model.ColorSettings;
        var highlightSettings = _model.HighlightSettings;

        var offset = sizeSettings.MountingRimSize + sizeSettings.PaintingMargin;
        var patternOrigin = new Point(paintingOrigin.X + offset, paintingOrigin.Y + offset);

        var highlights =
            highlightSettings.Highlights
                .GroupBy(highlight => highlight.Position)
                .ToDictionary(group => group.Key, g => g.Last().Color);

        for (var row = -1; row <= sizeSettings.GridRows; row++)
        {
            for (var col = -1; col <= sizeSettings.GridColumns; col++)
            {
                if (!highlights.TryGetValue(new Point(row, col), out var color))
                    color = colorSettings.DiamondColor;

                var centerX =
                    patternOrigin.X +
                    sizeSettings.OffsetX +
                    col * sizeSettings.DiamondWidth +
                    sizeSettings.DiamondWidth * .5;

                var centerY =
                    patternOrigin.Y +
                    sizeSettings.OffsetY +
                    row * sizeSettings.DiamondHeight +
                    sizeSettings.DiamondHeight * .5;

                var diamond = new Diamond(new Point(centerX, centerY), sizeSettings.DiamondSize, color)
                {
                    Clip = new RectangleGeometry(new Rect(patternOrigin, sizeSettings.PatternSize))
                };

                var row1 = row;
                var col1 = col;
                diamond.Clicked += () => { OpenColorPicker(row1, col1, color); };
                diamond.RightClicked += () =>
                {
                    RemoveHighlight(row1, col1);
                    Render();
                };

                MainCanvas.Children.Add(diamond.Shape);
            }
        }
    }

    private void RemoveHighlight(int row, int col)
    {
        var highlightSettings = _model.HighlightSettings;

        highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == new Point(row, col));
    }

    private void GetDiamondTicks(SizeSettings size, out double[] horizontalTicks, out double[] verticalTicks)
    {
        var displaySettings = _model.DisplaySettings;

        verticalTicks = new double[size.GridRows];
        horizontalTicks = new double[size.GridColumns];

        var offset = displaySettings.OnlyPattern
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
        var highlightSettings = _model.HighlightSettings;

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
            highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == position);
            var newHighlight = new Highlight(position, picker.SelectedColor ?? initialColor, false);
            highlightSettings.Highlights.Add(newHighlight);
            Render();
        };

        button.Click += (_, _) => { dialog.Close(); };

        dialog.Show();
    }

    private void RenderFrame(Point paintingOrigin)
    {
        if (_model.DisplaySettings.ShowExplodedFrame)
            RenderExplodedFrame(paintingOrigin);
        else
            RenderCompleteFrame(paintingOrigin);
    }

    private void RenderExplodedFrame(Point paintingOrigin)
    {
        MainCanvas.Children.Add(new Ellipse { Width = 50, Height = 50, Fill = new SolidColorBrush(Colors.Aqua) });
    }

    private void RenderCompleteFrame(Point paintingOrigin)
    {
        var structureWidth = _model.FrameSizeSettings.StructuralLayerWidth;
        var paintingSize = _model.SizeSettings.PaintingSize;
        var wiggleRoom = _model.FrameSizeSettings.WiggleRoom;

        var outerFramePerimeter = new Rect(
            paintingOrigin.X - (structureWidth + wiggleRoom),
            paintingOrigin.Y - (structureWidth + wiggleRoom),
            paintingSize.Width +
            2 * structureWidth +
            2 * wiggleRoom,
            paintingSize.Height +
            2 * structureWidth +
            2 * wiggleRoom
        );

        if (_model.DisplaySettings.ShowDebugLines)
        {
            var frameOutline = new Rectangle
            {
                Width = outerFramePerimeter.Width,
                Height = outerFramePerimeter.Height,
                Stroke = new SolidColorBrush(MyColors.Debug)
            };

            MainCanvas.Children.Add(frameOutline.WithOrigin(outerFramePerimeter.Location, ZIndex.Debug));
        }

        var horizontalStructureTop = CreateStructuralHorizontalPiece();
        var horizontalStructureBottom = CreateStructuralHorizontalPiece();

        MainCanvas.Children.Add(horizontalStructureTop.WithOrigin(outerFramePerimeter.Location));

        horizontalStructureBottom.WithOrigin(
            outerFramePerimeter.Left,
            paintingOrigin.Y + paintingSize.Height + wiggleRoom);
        MainCanvas.Children.Add(horizontalStructureBottom);

        var verticalStructureLeft = CreateStructuralVerticalPiece();
        var verticalStructureRight = CreateStructuralVerticalPiece();

        verticalStructureLeft.WithOrigin(
            outerFramePerimeter.Left,
            paintingOrigin.Y - wiggleRoom);
        MainCanvas.Children.Add(verticalStructureLeft);

        verticalStructureRight.WithOrigin(
            paintingOrigin.X + wiggleRoom + paintingSize.Width,
            paintingOrigin.Y - wiggleRoom);
        MainCanvas.Children.Add(verticalStructureRight);
    }

    private Shape CreateStructuralHorizontalPiece()
    {
        var dimensions = _model.FrameSizeSettings;
        var colors = _model.FrameColorSettings;

        return new Rectangle
        {
            Height = dimensions.StructuralLayerWidth,
            Width = _model.SizeSettings.PaintingSize.Width +
                    2 * dimensions.StructuralLayerWidth +
                    2 * dimensions.WiggleRoom,
            Stroke = new SolidColorBrush(colors.StructuralLayerColor),
            StrokeThickness = 2
        };
    }

    private Shape CreateStructuralVerticalPiece()
    {
        var dimensions = _model.FrameSizeSettings;
        var colors = _model.FrameColorSettings;

        return new Rectangle
        {
            Height = _model.SizeSettings.PaintingSize.Height + 2 * dimensions.WiggleRoom,
            Width = dimensions.StructuralLayerWidth,
            Stroke = new SolidColorBrush(colors.StructuralLayerColor),
            StrokeThickness = 3
        };
    }
}