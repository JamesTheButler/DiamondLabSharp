using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Model;
using Diamonds.Utilities;

namespace Diamonds.Rendering;

public sealed class FrameRenderer(ApplicationModel model, Canvas canvas)
{
    private const int StructureZIndex = 1;
    private const int Decorative1ZIndex = 2;
    private const int Decorative2ZIndex = 3;

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
        var structureWidth = frameSizes.StructuralLayerWidth;
        var wiggleRoom = frameSizes.WiggleRoom;
        var paintingSize = model.SizeSettings.PaintingSize;

        var outerPerimeter = new Rect(
            paintingOrigin.X - (structureWidth + wiggleRoom),
            paintingOrigin.Y - (structureWidth + wiggleRoom),
            paintingSize.Width +
            2 * structureWidth +
            2 * wiggleRoom,
            paintingSize.Height +
            2 * structureWidth +
            2 * wiggleRoom
        );

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

        var horizontalStructureTop = CreateStructuralHorizontalPiece();
        var horizontalStructureBottom = CreateStructuralHorizontalPiece();

        canvas.Children.Add(horizontalStructureTop.WithOrigin(outerPerimeter.Location, StructureZIndex));

        horizontalStructureBottom.WithOrigin(
            outerPerimeter.Left,
            paintingOrigin.Y + paintingSize.Height + wiggleRoom,
            StructureZIndex);
        canvas.Children.Add(horizontalStructureBottom);

        var verticalStructureLeft = CreateStructuralVerticalPiece();
        var verticalStructureRight = CreateStructuralVerticalPiece();

        verticalStructureLeft.WithOrigin(
            outerPerimeter.Left,
            paintingOrigin.Y - wiggleRoom,
            StructureZIndex);
        canvas.Children.Add(verticalStructureLeft);

        verticalStructureRight.WithOrigin(
            paintingOrigin.X + wiggleRoom + paintingSize.Width,
            paintingOrigin.Y - wiggleRoom,
            StructureZIndex);
        canvas.Children.Add(verticalStructureRight);

        RenderDecorativeLayers(outerPerimeter);
    }


    private Shape CreateStructuralHorizontalPiece()
    {
        var dimensions = model.FrameSizeSettings;
        var colors = model.FrameColorSettings;

        return new Rectangle
        {
            Height = dimensions.StructuralLayerWidth,
            Width = model.SizeSettings.PaintingSize.Width +
                    2 * dimensions.StructuralLayerWidth +
                    2 * dimensions.WiggleRoom,
            Stroke = new SolidColorBrush(colors.StructuralLayerColor),
            StrokeThickness = 1
        };
    }

    private Shape CreateStructuralVerticalPiece()
    {
        var dimensions = model.FrameSizeSettings;
        var colors = model.FrameColorSettings;

        return new Rectangle
        {
            Height = model.SizeSettings.PaintingSize.Height + 2 * dimensions.WiggleRoom,
            Width = dimensions.StructuralLayerWidth,
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
        var layer = new Polygon[4];

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

        layer[0] = top;
        layer[1] = left;
        layer[2] = bottom;
        layer[3] = right;

        return layer;
    }
}