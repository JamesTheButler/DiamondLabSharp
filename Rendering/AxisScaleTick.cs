namespace Diamonds.Rendering;

public readonly record struct AxisScaleTick(double Position)
{
    public ITickLabel Label { get; }
    public double? SizeOverride { get; }
    
    public AxisScaleTick(double position,
        ITickLabel? Label = null,
        double? SizeOverride = null) 
        : this(position)
    {
        Position = position;
        this.Label = Label ?? new ValueLabel();
        this.SizeOverride = SizeOverride;
    }

    public void Deconstruct(out double position, out ITickLabel label, out double? sizeOverride)
    {
        position = Position;
        label = Label;
        sizeOverride = SizeOverride;
    }
}

public interface ITickLabel
{
    string Render(AxisScaleTick tick);
}

public sealed class ValueLabel : ITickLabel
{
    public string Render(AxisScaleTick tick) => tick.Position.ToString("0.#");
}

public sealed class StaticLabel(string label) : ITickLabel
{
    public string Render(AxisScaleTick tick) => label;
}

public sealed class FormatterLabel(Func<double, string> formatter) : ITickLabel
{
    public string Render(AxisScaleTick tick) => formatter.Invoke(tick.Position);
}