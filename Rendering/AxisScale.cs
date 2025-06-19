using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Diamonds.Rendering;

public sealed class AxisScale(
    double length,
    Orientation orientation,
    AxisScaleTick[] ticks,
    Brush stroke,
    double strokeThickness,
    double fontSize,
    double defaultTickSize,
    double endTickSize)
    : UIElement
{
    private const double LabelMargin = 4.0;
    
    private readonly Typeface _typeface = new("Segoe UI");
    
    public AxisScale(double length, Orientation orientation)
    :this(
        length,
        orientation, 
        ticks: [])
    {
        
    }
    
    public AxisScale(double length, Orientation orientation, AxisScaleTick[] ticks)
    :this(
        length,
        orientation, 
        ticks,
        stroke: new SolidColorBrush(Colors.Black), 
        strokeThickness: 2.0,
        fontSize: 12.0, 
        defaultTickSize: 5.0, 
        endTickSize: 10.0 )
    {
        
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (orientation == Orientation.Horizontal)
        {
            RenderHorizontalScale(drawingContext);
        }
        else
        {
            RenderVerticalScale(drawingContext);
        }
        
    }

    private void RenderVerticalScale(DrawingContext drawingContext)
    {
        var pen = new Pen(stroke, strokeThickness);
        // scale line
        drawingContext.DrawLine(pen, new Point(0, 0), new Point(0, length));
        
        foreach (var (pos, label, sizeOverride) in ticks.Union(EndTicks))
        {
            var tickLength = sizeOverride ?? defaultTickSize;
            drawingContext.DrawLine(pen, new Point(0, pos), new Point(tickLength, pos));

            var tickLabel = new FormattedText(
                label,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                _typeface,
                fontSize,
                stroke,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            drawingContext.DrawText(tickLabel, new Point(tickLength + LabelMargin, pos - tickLabel.Height / 2));
        }
    }

    private void RenderHorizontalScale(DrawingContext drawingContext)
    {
        var pen = new Pen(stroke, strokeThickness);
        // scale line
        drawingContext.DrawLine(pen, new Point(0, 0), new Point(length, 0));
        
        foreach (var (pos, label, sizeOverride) in ticks.Union(EndTicks))
        {
            var tickLength = sizeOverride ?? defaultTickSize;
            drawingContext.DrawLine(pen, new Point(pos, 0), new Point(pos, tickLength));
            
            var tickLabel = new FormattedText(
                label,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                _typeface,
                fontSize,
                stroke,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );
            

            drawingContext.DrawText(tickLabel, new Point(pos - tickLabel.Width / 2, tickLength + LabelMargin));
        }
    }
    
    private AxisScaleTick[] EndTicks =>
    [
        new (0, "0", endTickSize),
        new (length, length.ToString("0.0"), endTickSize)
    ];
}