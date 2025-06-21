using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Rendering;
using Diamonds.Rendering.AxisScale;
using Diamonds.Utilities;
using Microsoft.Win32;

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