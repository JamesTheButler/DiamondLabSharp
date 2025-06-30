using System.IO;

namespace Diamonds.Model;

public sealed class ApplicationModel
{
    public string? ActiveFilePath { get; set; }
    public string? ActiveFileName => Path.GetFileNameWithoutExtension(ActiveFilePath);

    public ColorSettings ColorSettings { get; set; }
    public DisplaySettings DisplaySettings { get; set; }
    public HighlightSettings HighlightSettings { get; set; }
    public SizeSettings SizeSettings { get; set; }

    public ApplicationModel()
    {
        ResetSizes();
        ResetColors();
        ResetDisplaySettings();
        ResetHighlights();
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
}