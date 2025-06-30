namespace Diamonds.Model;

public readonly record struct DisplaySettings(bool ShowScales, bool OnlyPattern)
{
    public static DisplaySettings Defaults => new(true, false);
}