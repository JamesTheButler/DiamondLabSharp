using System.Windows;

namespace Diamonds.Rendering;

/// <summary>
/// Pattern dimensions in mm.
/// </summary>
public readonly record struct SizeSettings(
    int DiamondWidth,
    int DiamondHeight,
    int GridColumns,
    int GridRows,
    int PaintingMargin,
    int MountingRimSize)
{
    public Size PaintingSize => GetTotalSize();
    
    public static SizeSettings Defaults => new(
        DiamondWidth: 60,
        DiamondHeight: 100,
        GridColumns: 10,
        GridRows: 4,
        PaintingMargin: 20,
        MountingRimSize: 30);
    
    private Size GetTotalSize()
    {
        var width = GridColumns * DiamondWidth + 2 * (PaintingMargin + MountingRimSize);
        var height = GridRows * DiamondHeight + 2 * (PaintingMargin + MountingRimSize);
        return new Size(width, height);
    }
    
}