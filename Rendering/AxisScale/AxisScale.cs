using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiamondLab.Rendering.AxisScale;

public sealed class AxisScale : UIElement
{
    private const double LabelMargin = 4.0;

    private readonly Typeface _typeface = new("Segoe UI");

    public double Length { get; }
    public Orientation Orientation { get; }
    public AxisScaleTick[] Ticks { get; init; } = [];
    public Brush Stroke { get; init; } = new SolidColorBrush(Colors.Black);
    public double StrokeThickness { get; init; } = 2.0;
    public double FontSize { get; init; } = 12;
    public double DefaultTickSize { get; init; } = 4.0;
    public double EndTickSize { get; init; } = 8.0;

    public AxisScale(double length, Orientation orientation)
    {
        Length = length;
        Orientation = orientation;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (Orientation == Orientation.Horizontal)
            RenderHorizontalScale(drawingContext);
        else
            RenderVerticalScale(drawingContext);
    }

    private void RenderVerticalScale(DrawingContext drawingContext)
    {
        var pen = new Pen(Stroke, StrokeThickness);
        // scale line
        drawingContext.DrawLine(pen, new Point(0, 0), new Point(0, Length));

        foreach (var tick in Ticks.Union(EndTicks))
        {
            var (pos, label, sizeOverride) = tick;

            var tickLength = sizeOverride ?? DefaultTickSize;
            drawingContext.DrawLine(pen, new Point(0, pos), new Point(tickLength, pos));

            var tickLabel = new FormattedText(
                label.Render(tick),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                _typeface,
                FontSize,
                Stroke,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            drawingContext.DrawText(tickLabel, new Point(tickLength + LabelMargin, pos - tickLabel.Height / 2));
        }
    }

    private void RenderHorizontalScale(DrawingContext drawingContext)
    {
        var pen = new Pen(Stroke, StrokeThickness);
        // scale line
        drawingContext.DrawLine(pen, new Point(0, 0), new Point(Length, 0));

        foreach (var tick in Ticks.Union(EndTicks))
        {
            var (pos, label, sizeOverride) = tick;

            var tickLength = sizeOverride ?? DefaultTickSize;
            drawingContext.DrawLine(pen, new Point(pos, 0), new Point(pos, tickLength));

            var tickLabel = new FormattedText(
                label.Render(tick),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                _typeface,
                FontSize,
                Stroke,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );


            drawingContext.DrawText(tickLabel, new Point(pos - tickLabel.Width / 2, tickLength + LabelMargin));
        }
    }

    private AxisScaleTick[] EndTicks =>
    [
        new(0, SizeOverride: EndTickSize),
        new(Length, SizeOverride: EndTickSize)
    ];
}