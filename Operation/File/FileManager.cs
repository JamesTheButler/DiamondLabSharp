using System.IO;
using System.Windows.Controls;
using Diamonds.Model;
using Diamonds.Operation.Notification;
using Diamonds.Utilities;
using Microsoft.Win32;

namespace Diamonds.Operation.File;

public sealed class FileManager : IFileManager
{
    private readonly ApplicationModel _model;
    private readonly INotificationManager _notificationManager;

    public FileManager(ApplicationModel model, INotificationManager notificationManager)
    {
        _model = model;
        _notificationManager = notificationManager;

        if (model.ActiveFilePath is not null)
            Load(model.ActiveFilePath);
    }

    public void QuickSave()
    {
        var saveData = GetSerializedData();

        if (_model.ActiveFilePath is not null)
        {
            var quickSaveResult = FileOperations.Save(saveData, _model.ActiveFilePath);

            if (quickSaveResult)
            {
                _notificationManager.PostNotification(new SaveSuccessNotification(_model.ActiveFilePath));
                return;
            }
        }

        var dialog = OpenSaveFileDialog(FileManagementDefaults.FileName, FileManagementDefaults.DefaultLocation);
        if (dialog.ShowDialog() != true)
            return;

        var fileName = dialog.FileName;
        var saveResult = FileOperations.Save(saveData, fileName);

        if (saveResult)
            _model.ActiveFilePath = fileName;

        INotification notification = saveResult
            ? new SaveSuccessNotification(fileName)
            : new SaveFailureNotification(fileName);

        _notificationManager.PostNotification(notification);
    }

    public void SaveToPng(Canvas canvas)
    {
        var dialogFileName = _model.ActiveFileName ?? FileManagementDefaults.FileName;
        var dialogDirectory = GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        var dialog = new SaveFileDialog
        {
            Filter = "PNG Files (*.png)|*.png",
            DefaultExt = ".png",
            FileName = $"{dialogFileName}.png",
            DefaultDirectory = dialogDirectory
        };

        if (dialog.ShowDialog() != true)
            return;

        FileOperations.SaveAsPng(canvas, dialog.FileName);
        _notificationManager.PostNotification(new SaveSuccessNotification(dialog.FileName));
    }

    public void Save()
    {
        var dialogFileName = _model.ActiveFileName ?? FileManagementDefaults.FileName;
        var dialogDirectory = GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        var dialog = OpenSaveFileDialog(dialogFileName, dialogDirectory);
        if (dialog.ShowDialog() != true)
            return;

        var saveData = GetSerializedData();
        var filePath = dialog.FileName;
        var saveResult = FileOperations.Save(saveData, filePath);

        if (saveResult)
            _model.ActiveFilePath = filePath;
    }

    public bool Load()
    {
        var dialogDirectory = GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        const string extension = FileManagementDefaults.FileExtension;
        var dialog = new OpenFileDialog
        {
            Filter = $"{extension} Files (*.{extension})|*.{extension}",
            DefaultExt = $".{extension}",
            DefaultDirectory = dialogDirectory
        };

        if (dialog.ShowDialog() != true)
            return false;

        var filePath = dialog.FileName;
        var loadResult = Load(filePath);
        return loadResult;
    }

    public bool Load(string filePath)
    {
        var data = FileOperations.Load(filePath);
        if (data is null)
        {
            _notificationManager.PostNotification(new LoadFailureNotification(filePath));
            return false;
        }

        _model.ActiveFilePath = filePath;
        _model.SizeSettings = data.Value.SizeSettings;
        _model.ColorSettings = data.Value.ColorSettings;
        _model.DisplaySettings = data.Value.DisplaySettings;
        _model.HighlightSettings = data.Value.HighlightSettings;
        _model.FrameSizeSettings = data.Value.FrameSizeSettings;
        _model.FrameColorSettings = data.Value.FrameColorSettings;

        _notificationManager.PostNotification(new LoadSuccessNotification(filePath));
        return true;
    }

    private static SaveFileDialog OpenSaveFileDialog(string fileName, string filePath)
    {
        const string extension = FileManagementDefaults.FileExtension;

        var dialog = new SaveFileDialog
        {
            Filter = $"{extension} Files (*.{extension})|*.{extension}",
            DefaultExt = $".{extension}",
            FileName = $"{fileName}.{extension}",
            DefaultDirectory = filePath
        };

        return dialog;
    }

    private string? GetActiveFileLocation()
    {
        return Path.GetDirectoryName(_model.ActiveFilePath);
    }

    private SerializedData GetSerializedData()
    {
        return new SerializedData(
            _model.SizeSettings,
            _model.ColorSettings,
            _model.DisplaySettings,
            _model.HighlightSettings,
            _model.FrameSizeSettings,
            _model.FrameColorSettings);
    }
}