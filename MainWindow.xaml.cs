using System.Windows;
using Diamonds.Model;
using Diamonds.Utilities;

namespace Diamonds;

public partial class MainWindow
{
    private const int InfoBarThickness = 50;
    private readonly Thickness _canvasMargin = new(10, 10, 0, 0);

    private readonly FileManager _fileManager = FileManager.Instance;

    private readonly Thickness _paintingMargin = new(10);
    private ColorSettings _colorSettings = ColorSettings.Defaults;
    private DisplaySettings _displaySettings = DisplaySettings.Defaults;
    private HighlightSettings _highlightSettings = new([]);

    private SizeSettings _sizeSettings = SizeSettings.Defaults;

    private Point PaintingOrigin;

    public MainWindow() : this(null)
    {
    }

    public MainWindow(string? startupFilePath)
    {
        InitializeComponent();
        SetUpUi();
        InitializeNotificationTimer();

        DataContext = this;
        if (startupFilePath != null)
            LoadFile(startupFilePath);

        ReDraw();
    }
}