namespace Diamonds.Model;

public readonly record struct FrameSizeSettings(
    int StructuralLayerWidth,
    int DecorativeLayer1Width,
    int DecorativeLayer2Width,
    int WiggleRoom)
{
    public static FrameSizeSettings Defaults => new(
        15,
        30,
        20,
        3
    );
}