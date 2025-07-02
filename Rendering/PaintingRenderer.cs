using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Model;
using Diamonds.Utilities;
using Xceed.Wpf.Toolkit;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

namespace Diamonds.Rendering;

public sealed class PaintingRenderer(ApplicationModel model, Canvas mainCanvas, Action onRenderDataChanged)
{
    public void Render(Point paintingOrigin)
    {
        RenderMountingRim(paintingOrigin);
        RenderPaintingBackground(paintingOrigin);
        RenderDiamondPattern(paintingOrigin);
    }

    private void RenderMountingRim(Point origin)
    {
        var sizeSettings = model.SizeSettings;
        var colorSettings = model.ColorSettings;

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

        mainCanvas.Children.Add(mountingRim.WithOrigin(origin));

        var mountingRimOutline = new Rectangle
        {
            Width = paintingSize.Width - 2 * rim,
            Height = paintingSize.Height - 2 * rim,
            Stroke = new SolidColorBrush(colorSettings.MountingRimColor),
        };
        mainCanvas.Children.Add(mountingRimOutline.WithOrigin(origin.Add(rim)));
    }

    private void RenderPaintingBackground(Point origin)
    {
        var sizeSettings = model.SizeSettings;
        var colorSettings = model.ColorSettings;

        var offset = sizeSettings.MountingRimSize + sizeSettings.PaintingMargin;
        var paintedBackground = new Rectangle
        {
            Width = sizeSettings.GridColumns * sizeSettings.DiamondWidth,
            Height = sizeSettings.GridRows * sizeSettings.DiamondHeight,
            Fill = new SolidColorBrush(colorSettings.BackgroundColor)
        };
        mainCanvas.Children.Add(paintedBackground.WithOrigin(origin.Add(offset)));
    }

    private void RenderDiamondPattern(Point paintingOrigin)
    {
        var sizeSettings = model.SizeSettings;
        var colorSettings = model.ColorSettings;
        var highlightSettings = model.HighlightSettings;

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
                    onRenderDataChanged.Invoke();
                };

                mainCanvas.Children.Add(diamond.Shape);
            }
        }
    }

    private void RemoveHighlight(int row, int col)
    {
        var highlightSettings = model.HighlightSettings;

        highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == new Point(row, col));
    }

    private void OpenColorPicker(int highlightRow, int highlightCol, Color initialColor)
    {
        var highlightSettings = model.HighlightSettings;

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
            Owner = mainCanvas.Parent as Window,
            ResizeMode = ResizeMode.NoResize
        };

        picker.SelectedColorChanged += (_, _) =>
        {
            var position = new Point(highlightRow, highlightCol);
            highlightSettings.Highlights.RemoveAll(highlight => highlight.Position == position);
            var newHighlight = new Highlight(position, picker.SelectedColor ?? initialColor, false);
            highlightSettings.Highlights.Add(newHighlight);
            onRenderDataChanged.Invoke();
        };

        button.Click += (_, _) => { dialog.Close(); };

        dialog.Show();
    }
}