namespace DiamondLab.Model;

public readonly record struct SerializedData(
    SizeSettings SizeSettings,
    ColorSettings ColorSettings,
    DisplaySettings DisplaySettings,
    HighlightSettings HighlightSettings,
    FrameSizeSettings FrameSizeSettings,
    FrameColorSettings FrameColorSettings);