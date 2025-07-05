using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DiamondLab.Model;
using DiamondLab.Utilities;

namespace DiamondLab.Rendering;

public sealed class FrameRenderer(ApplicationModel model, Canvas canvas)
{
    private const int StructureZIndex = 1;
    private const int Decorative1ZIndex = 2;
    private const int Decorative2ZIndex = 3;
    private const int LabelMargin = 5;

    public void Render(Point paintingOrigin)
    {
        if (model.DisplaySettings.ShowExplodedFrame)
            RenderExplodedFrame(paintingOrigin);
        else
            RenderCompleteFrame(paintingOrigin);
    }

    private void RenderExplodedFrame(Point paintingOrigin)
    {
        canvas.Children.Add(
            new Ellipse { Width = 50, Height = 50, Fill = new SolidColorBrush(MyColors.Debug) }
                .WithOrigin(paintingOrigin));
    }

    private void RenderCompleteFrame(Point paintingOrigin)
    {
        var frameSizes = model.FrameSizeSettings;
        var paintingSizes = model.SizeSettings;

        var outerPerimeter = new Rect(
            paintingOrigin.Add(-frameSizes.DecorativeLayer1Width + paintingSizes.MountingRimSize),
            paintingSizes.PaintingSize.Add(2 * (frameSizes.DecorativeLayer1Width - paintingSizes.MountingRimSize))
        );

        RenderStructure(outerPerimeter);
        RenderDecorativeLayers(outerPerimeter);
        RenderPerimeterSizeLabels(outerPerimeter);
    }

    private void RenderPerimeterSizeLabels(Rect outerPerimeter)
    {
        var widthLabel = new TextBlock
        {
            Text = outerPerimeter.Width.ToString(CultureInfo.InvariantCulture),
            FontSize = 16,
            Foreground = new SolidColorBrush(model.FrameColorSettings.StructuralLayerColor.Darken()),
            Background = Brushes.Transparent
        };

        widthLabel.WithOrigin(
            outerPerimeter.TopLeft + new Vector(
                outerPerimeter.Width / 2,
                -(widthLabel.FontSize + LabelMargin)));

        canvas.Children.Add(widthLabel);

        var heightLabel = new TextBlock
        {
            Text = outerPerimeter.Height.ToString(CultureInfo.InvariantCulture),
            FontSize = 16,
            Foreground = new SolidColorBrush(model.FrameColorSettings.StructuralLayerColor.Darken()),
            Background = Brushes.Transparent
        };

        heightLabel.WithOrigin(
            outerPerimeter.TopLeft + new Vector(
                -(widthLabel.FontSize + 20),
                outerPerimeter.Height / 2));

        canvas.Children.Add(heightLabel);
    }

    private void RenderStructure(Rect outerPerimeter)
    {
        var structureWidth = model.FrameSizeSettings.StructuralLayerWidth;
        if (model.DisplaySettings.ShowDebugLines)
        {
            var frameOutline = new Rectangle
            {
                Width = outerPerimeter.Width,
                Height = outerPerimeter.Height,
                Stroke = new SolidColorBrush(MyColors.Debug)
            };

            canvas.Children.Add(frameOutline.WithOrigin(outerPerimeter.Location, ZIndex.Debug));
        }

        // horizontal top
        var horizontalStructureTop = CreateStructuralHorizontalPiece(outerPerimeter);
        canvas.Children.Add(horizontalStructureTop.WithOrigin(outerPerimeter.Location, StructureZIndex));

        // horizontal bottom
        var horizontalStructureBottom = CreateStructuralHorizontalPiece(outerPerimeter);
        horizontalStructureBottom.WithOrigin(
            outerPerimeter.BottomLeft - new Vector(0, structureWidth),
            StructureZIndex);
        canvas.Children.Add(horizontalStructureBottom);

        // vertical left
        var verticalStructureLeft = CreateStructuralVerticalPiece(outerPerimeter);
        verticalStructureLeft.WithOrigin(
            outerPerimeter.TopLeft + new Vector(0, structureWidth),
            StructureZIndex);
        canvas.Children.Add(verticalStructureLeft);

        // vertical right
        var verticalStructureRight = CreateStructuralVerticalPiece(outerPerimeter);
        verticalStructureRight.WithOrigin(
            outerPerimeter.TopRight + new Vector(-structureWidth, structureWidth),
            StructureZIndex);
        canvas.Children.Add(verticalStructureRight);
    }

    private Rectangle CreateStructuralHorizontalPiece(Rect outerPerimeter)
    {
        var dimensions = model.FrameSizeSettings;
        var colors = model.FrameColorSettings;

        return new Rectangle
        {
            Height = dimensions.StructuralLayerWidth,
            Width = outerPerimeter.Width,
            Stroke = new SolidColorBrush(colors.StructuralLayerColor),
            StrokeThickness = 1
        };
    }

    private Rectangle CreateStructuralVerticalPiece(Rect outerPerimeter)
    {
        var frameSizes = model.FrameSizeSettings;
        var paintingSizes = model.FrameSizeSettings;
        var colors = model.FrameColorSettings;

        return new Rectangle
        {
            Height = outerPerimeter.Height - 2 * paintingSizes.StructuralLayerWidth,
            Width = paintingSizes.StructuralLayerWidth,
            Stroke = new SolidColorBrush(colors.StructuralLayerColor),
            StrokeThickness = 1
        };
    }

    private void RenderDecorativeLayers(Rect outerPerimeter)
    {
        var frameSizes = model.FrameSizeSettings;
        var frameColors = model.FrameColorSettings;
        var layer1 = CreateDecorativeLayer(outerPerimeter, frameSizes.DecorativeLayer1Width, Decorative1ZIndex,
            frameColors.DecorativeLayer1Color);
        var layer2 = CreateDecorativeLayer(outerPerimeter, frameSizes.DecorativeLayer2Width, Decorative2ZIndex,
            frameColors.DecorativeLayer2Color);

        foreach (var shape in layer1.Union(layer2))
        {
            canvas.Children.Add(shape);
        }
    }

    private Polygon[] CreateDecorativeLayer(Rect outerPerimeter, int layerThickness, int zIndex, Color color)
    {
        if (layerThickness == 0)
            return [];

        var top = new Polygon
        {
            Points =
            {
                new Point(0, 0),
                new Point(outerPerimeter.Width, 0),
                new Point(outerPerimeter.Width - layerThickness, layerThickness),
                new Point(layerThickness, layerThickness)
            },
            Fill = new SolidColorBrush(color),
            Stroke = new SolidColorBrush(color.Darken())
        }.WithOrigin(outerPerimeter.TopLeft, zIndex);

        var bottom = new Polygon
        {
            Points =
            {
                new Point(0, 0),
                new Point(outerPerimeter.Width, 0),
                new Point(outerPerimeter.Width - layerThickness, -layerThickness),
                new Point(layerThickness, -layerThickness)
            },
            Fill = new SolidColorBrush(color),
            Stroke = new SolidColorBrush(color.Darken())
        }.WithOrigin(outerPerimeter.BottomLeft, zIndex);

        var left = new Polygon
        {
            Points =
            {
                new Point(0, 0),
                new Point(layerThickness, layerThickness),
                new Point(layerThickness, outerPerimeter.Height - layerThickness),
                new Point(0, outerPerimeter.Height)
            },
            Fill = new SolidColorBrush(color),
            Stroke = new SolidColorBrush(color.Darken())
        }.WithOrigin(outerPerimeter.TopLeft, zIndex);

        var right = new Polygon
        {
            Points =
            {
                new Point(0, 0),
                new Point(-layerThickness, layerThickness),
                new Point(-layerThickness, outerPerimeter.Height - layerThickness),
                new Point(0, outerPerimeter.Height)
            },
            Fill = new SolidColorBrush(color),
            Stroke = new SolidColorBrush(color.Darken())
        }.WithOrigin(outerPerimeter.TopRight, zIndex);


        return
        [
            top,
            left,
            bottom,
            right
        ];
    }
}