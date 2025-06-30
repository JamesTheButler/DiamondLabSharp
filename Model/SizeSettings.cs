using System.Windows;
using Diamonds.Utilities;

namespace Diamonds.Model;

/// <summary>
/// Pattern dimensions in mm.
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
    public Thickness TotalMargin => new (PaintingMargin + MountingRimSize);
    public Size PatternSize => new(GridColumns * DiamondWidth, GridRows * DiamondHeight);
    public Size PaintingSize => PatternSize.Add(TotalMargin);

    public static SizeSettings Defaults => new(
        DiamondWidth: 60,
        DiamondHeight: 100,
        GridColumns: 10,
        GridRows: 4,
        PaintingMargin: 20,
        MountingRimSize: 30,
        OffsetX: 0,
        OffsetY: 0);
}