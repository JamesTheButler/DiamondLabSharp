using System.Windows;
using Diamonds.Rendering;
using Diamonds.Utilities;

namespace Diamonds;

public partial class MainWindow
{
    private const int InfoBarThickness = 50;

    private readonly FileManager _fileManager = FileManager.Instance;

    private SizeSettings _currentSizeSettings = SizeSettings.Defaults;
    private ColorSettings _currentColorSettings = ColorSettings.Defaults;

    private readonly Thickness _paintingMargin = new(10);
    private readonly Thickness _canvasMargin = new(10, 10, 0, 0);

    public MainWindow() : this(null)
    {
    }

    public MainWindow(string? startupFilePath)
    {
        InitializeComponent();
        SetUpUi();

        if (startupFilePath != null)
            LoadFile(startupFilePath);

        ReDraw();
    }
}