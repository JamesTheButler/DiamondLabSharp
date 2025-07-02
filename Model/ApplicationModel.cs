using System.IO;
using System.Text.Json;
using System.Windows;

namespace Diamonds.Model;

public sealed class ApplicationModel
{
    public string? ActiveFilePath
    {
        get => _activeFilePath;
        set
        {
            _activeFilePath = value;
            ActiveFileName = Path.GetFileNameWithoutExtension(value);
            CacheActiveFilePath();
        }
    }

    public string? ActiveFileName { get; private set; }

    public ColorSettings ColorSettings { get; set; }
    public DisplaySettings DisplaySettings { get; set; }
    public HighlightSettings HighlightSettings { get; set; }
    public SizeSettings SizeSettings { get; set; }
    public FrameSizeSettings FrameSizeSettings { get; set; }
    public FrameColorSettings FrameColorSettings { get; set; }

    public double[] HorizontalDiamondTicks => GetHorizontalDiamondTicks();
    public double[] VerticalDiamondTicks => GetVerticalDiamondTicks();

    private string? _activeFilePath;

    public ApplicationModel()
    {
        ResetSizes();
        ResetColors();
        ResetDisplaySettings();
        ResetHighlights();
        ResetFrameDimensions();
        ResetFrameColors();

        LoadActiveFilePathCache();
    }

    public void ResetSizes()
    {
        SizeSettings = SizeSettings.Defaults;
    }

    public void ResetColors()
    {
        ColorSettings = ColorSettings.Defaults;
    }

    public void ResetDisplaySettings()
    {
        DisplaySettings = DisplaySettings.Defaults;
    }

    public void ResetHighlights()
    {
        HighlightSettings = new HighlightSettings([]);
    }

    public void ResetFrameDimensions()
    {
        FrameSizeSettings = FrameSizeSettings.Defaults;
    }

    public void ResetFrameColors()
    {
        FrameColorSettings = FrameColorSettings.Defaults;
    }

    private void LoadActiveFilePathCache()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            nameof(Diamonds));

        var settingsFile = Path.Combine(appDataPath, "cache.json");

        if (!File.Exists(settingsFile))
            return;

        var json = File.ReadAllText(settingsFile);
        _activeFilePath = JsonSerializer.Deserialize<string>(json);
    }

    private void CacheActiveFilePath()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            nameof(Diamonds));

        var settingsFile = Path.Combine(appDataPath, "cache.json");

        Directory.CreateDirectory(appDataPath);
        File.WriteAllText(settingsFile, JsonSerializer.Serialize(ActiveFilePath));
    }

    private double[] GetHorizontalDiamondTicks()
    {
        var displaySettings = DisplaySettings;
        var size = SizeSettings;

        var ticks = new double[size.GridColumns];

        var offset = displaySettings.OnlyPattern
            ? (Point)size.Offset
            : new Point(
                size.MountingRimSize + size.PaintingMargin + size.OffsetX,
                size.MountingRimSize + size.PaintingMargin + size.OffsetY);

        for (var col = 0; col < size.GridColumns; col++)
        {
            ticks[col] = offset.X + size.DiamondWidth * .5 + size.DiamondWidth * col;
        }

        return ticks;
    }

    private double[] GetVerticalDiamondTicks()
    {
        var size = SizeSettings;
        var displaySettings = DisplaySettings;

        var ticks = new double[size.GridRows];

        var offset = displaySettings.OnlyPattern
            ? (Point)size.Offset
            : new Point(
                size.MountingRimSize + size.PaintingMargin + size.OffsetX,
                size.MountingRimSize + size.PaintingMargin + size.OffsetY);

        for (var row = 0; row < size.GridRows; row++)
        {
            ticks[row] = offset.Y + size.DiamondHeight * .5 + size.DiamondHeight * row;
        }

        return ticks;
    }
}