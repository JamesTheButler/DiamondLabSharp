using System.Windows;

namespace Diamonds.Rendering;

public readonly record struct SizeSettings(
    double DiamondWidth,
    double DiamondHeight,
    int GridColumns,
    int GridRows,
    double CanvasMarginSize,
    double MountingRimSize)
{
    public Size PaintingSize => GetTotalSize();
    
    public static SizeSettings Defaults => new(
        DiamondWidth: 60,
        DiamondHeight: 100,
        GridColumns: 10,
        GridRows: 4,
        CanvasMarginSize: 20,
        MountingRimSize: 30);
    
    private Size GetTotalSize()
    {
        var width = GridColumns * DiamondWidth + 2 * (CanvasMarginSize + MountingRimSize);
        var height = GridRows * DiamondHeight + 2 * (CanvasMarginSize + MountingRimSize);
        return new Size(width, height);
    }
    
}