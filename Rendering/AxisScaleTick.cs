namespace Diamonds.Rendering;

public record struct AxisScaleTick(
    double Position,
    string Label, 
    double? SizeOverride = null);