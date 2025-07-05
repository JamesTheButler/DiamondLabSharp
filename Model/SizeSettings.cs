using System.Windows;
using DiamondLab.Utilities;

namespace DiamondLab.Model;

/// <summary>
///     Pattern dimensions in mm.
/// </summary>
public readonly record struct SizeSettings(
    int DiamondWidth,
    int DiamondHeight,
    int GridColumns,
    int GridRows,
    int PaintingMargin,
    int MountingRimSize,
    int OffsetX,
    int OffsetY)
{
    public Size DiamondSize => new(DiamondWidth, DiamondHeight);
    public Vector Offset => new(OffsetX, OffsetY);
    public Thickness TotalMargin => new(PaintingMargin + MountingRimSize);
    public Size PatternSize => new(GridColumns * DiamondWidth, GridRows * DiamondHeight);
    public Size PaintingSize => PatternSize.Add(TotalMargin);

    public static SizeSettings Defaults => new(
        60,
        100,
        10,
        4,
        20,
        30,
        0,
        0);
}