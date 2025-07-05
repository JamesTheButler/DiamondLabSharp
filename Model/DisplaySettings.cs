namespace DiamondLab.Model;

public readonly record struct DisplaySettings(
    bool ShowScales,
    bool OnlyPattern,
    bool ShowDebugLines,
    bool ShowFrame,
    bool ShowExplodedFrame)
{
    public static DisplaySettings Defaults => new(
        true,
        false,
        false,
        true,
        false);
}