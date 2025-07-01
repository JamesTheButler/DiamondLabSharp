namespace Diamonds.Model;

public readonly record struct DisplaySettings(bool ShowScales, bool OnlyPattern, bool ShowDebugLines)
{
    public static DisplaySettings Defaults => new(true, false, false);
}