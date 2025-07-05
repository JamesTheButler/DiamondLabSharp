namespace DiamondLab.Model;

public readonly record struct FrameSizeSettings(
    int StructuralLayerWidth,
    int DecorativeLayer1Width,
    int DecorativeLayer2Width)
{
    public static FrameSizeSettings Defaults => new(
        15,
        30,
        20
    );
}