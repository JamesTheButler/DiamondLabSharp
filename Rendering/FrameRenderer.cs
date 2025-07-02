using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Model;
using Diamonds.Utilities;

namespace Diamonds.Rendering;

public sealed class FrameRenderer(ApplicationModel model, Canvas canvas)
{
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
        var structureWidth = model.FrameSizeSettings.StructuralLayerWidth;
        var paintingSize = model.SizeSettings.PaintingSize;
        var wiggleRoom = model.FrameSizeSettings.WiggleRoom;

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

        if (model.DisplaySettings.ShowDebugLines)
        {
            var frameOutline = new Rectangle
            {
                Width = outerFramePerimeter.Width,
                Height = outerFramePerimeter.Height,
                Stroke = new SolidColorBrush(MyColors.Debug)
            };

            canvas.Children.Add(frameOutline.WithOrigin(outerFramePerimeter.Location, ZIndex.Debug));
        }

        var horizontalStructureTop = CreateStructuralHorizontalPiece();
        var horizontalStructureBottom = CreateStructuralHorizontalPiece();

        canvas.Children.Add(horizontalStructureTop.WithOrigin(outerFramePerimeter.Location));

        horizontalStructureBottom.WithOrigin(
            outerFramePerimeter.Left,
            paintingOrigin.Y + paintingSize.Height + wiggleRoom);
        canvas.Children.Add(horizontalStructureBottom);

        var verticalStructureLeft = CreateStructuralVerticalPiece();
        var verticalStructureRight = CreateStructuralVerticalPiece();

        verticalStructureLeft.WithOrigin(
            outerFramePerimeter.Left,
            paintingOrigin.Y - wiggleRoom);
        canvas.Children.Add(verticalStructureLeft);

        verticalStructureRight.WithOrigin(
            paintingOrigin.X + wiggleRoom + paintingSize.Width,
            paintingOrigin.Y - wiggleRoom);
        canvas.Children.Add(verticalStructureRight);
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
            StrokeThickness = 2
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
            StrokeThickness = 3
        };
    }
}