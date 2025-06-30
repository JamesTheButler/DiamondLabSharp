using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Diamonds.Model;
using Diamonds.Rendering;
using Diamonds.Utilities;

namespace Diamonds;

public partial class MainWindow
{
    private const int InfoBarThickness = 50;

    private readonly FileManager _fileManager = FileManager.Instance;

    private SizeSettings _sizeSettings = SizeSettings.Defaults;
    private ColorSettings _colorSettings = ColorSettings.Defaults;
    private DisplaySettings _displaySettings = DisplaySettings.Defaults;
    private HighlightSettings _highlightSettings = new([]);

    private readonly Thickness _paintingMargin = new(10);
    private readonly Thickness _canvasMargin = new(10, 10, 0, 0);

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
        {
            LoadFile(startupFilePath);
        }

        ReDraw();
    }
}