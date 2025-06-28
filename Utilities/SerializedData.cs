using Diamonds.Rendering;

namespace Diamonds.Utilities;

public readonly record struct SerializedData(
    SizeSettings SizeSettings,
    ColorSettings ColorSettings,
    DisplaySettings DisplaySettings);