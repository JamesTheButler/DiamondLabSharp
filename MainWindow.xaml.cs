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

    private SizeSettings _currentSizeSettings = SizeSettings.Defaults;
    private ColorSettings _currentColorSettings = ColorSettings.Defaults;
    private DisplaySettings _currentDisplaySettings = DisplaySettings.Defaults;
    private HighlightSettings _highlightSettings = new([]);

    private readonly Thickness _paintingMargin = new(10);
    private readonly Thickness _canvasMargin = new(10, 10, 0, 0);

    public ObservableCollection<HighlightViewModel> Highlights { get; } = [];
    
    public MainWindow() : this(null)
    {
    }
    
    private void RemoveHighlight_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: HighlightViewModel vm })
            Highlights.Remove(vm);
    }

    private void AddHighlight_Click(object sender, RoutedEventArgs e) => AddHighlight();

    
    public MainWindow(string? startupFilePath)
    {
        InitializeComponent();
        SetUpUi();
        InitializeNotificationTimer();
        
        DataContext = this;
        Highlights.CollectionChanged += (_,_) => ReDraw();
        if (startupFilePath != null)
        {
            LoadFile(startupFilePath);
        }

        ReDraw();
    }

    private void AddHighlight()
    {
        var vm = new HighlightViewModel { Color = MyColors.Lightest };
        Highlights.Add(vm);
        vm.PropertyChanged += (_,_) => ReDraw();
    }

    private void RemoveHighlight(HighlightViewModel vm)
    {
        Highlights.Remove(vm);
    }
}