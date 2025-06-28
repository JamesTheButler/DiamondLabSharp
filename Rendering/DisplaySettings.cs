namespace Diamonds.Rendering;

public readonly record struct DisplaySettings(bool ShowScales)
{
    public static DisplaySettings Defaults => new(true);
}