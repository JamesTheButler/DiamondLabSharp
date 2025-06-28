using System.Windows;
using Diamonds.Utilities;

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
    int MountingRimSize,
    int OffsetX,
    int OffsetY)
{
    public Size MarginSize => GetMarginSize();
    public Size PaintingSize => GetTotalSize();
    public Size PatternSize => GetPatternSize();

    public static SizeSettings Defaults => new(
        DiamondWidth: 60,
        DiamondHeight: 100,
        GridColumns: 10,
        GridRows: 4,
        PaintingMargin: 20,
        MountingRimSize: 30,
        OffsetX: 0,
        OffsetY: 0);

    private Size GetMarginSize()
    {
        var totalMargin = 2 * (PaintingMargin + MountingRimSize);
        return new Size(totalMargin,totalMargin);
    }
    
    private Size GetPatternSize()
    {
        var width = GridColumns * DiamondWidth;
        var height = GridRows * DiamondHeight;
        return new Size(width, height);
    }
    
    private Size GetTotalSize()
    {
        return MarginSize.Add(PatternSize);
    }
}