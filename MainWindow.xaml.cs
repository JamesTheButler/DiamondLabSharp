using System.Windows;
using Diamonds.Model;
using Diamonds.Operation;
using Diamonds.Operation.File;
using Diamonds.Operation.Notification;
using Diamonds.Rendering;

namespace Diamonds;

public partial class MainWindow
{
    private const int InfoBarThickness = 50;

    private readonly ApplicationModel _model = Dependencies.Model;
    private readonly IFileManager _fileManager = Dependencies.FileManager;
    private readonly INotificationManager _notificationManager = Dependencies.NotificationManager;
    private readonly PaintingRenderer _paintingRenderer;
    private readonly FrameRenderer _frameRenderer;

    private readonly Thickness _paintingMargin = new(40);

    public MainWindow() : this(null)
    {
    }

    public MainWindow(string? startupFilePath)
    {
        InitializeComponent();
        _paintingRenderer = new PaintingRenderer(_model, MainCanvas, Render);
        _frameRenderer = new FrameRenderer(_model, MainCanvas);

        SetUpUi();

        InitializeNotificationTimer();
        _notificationManager.NotificationPosted += PostNotification;

        DataContext = this;
        if (startupFilePath != null)
            _fileManager.Load(startupFilePath);
    }
}