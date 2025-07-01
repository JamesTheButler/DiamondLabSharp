using System.IO;
using System.Windows.Controls;
using Diamonds.Model;
using Diamonds.Operation.Notification;
using Diamonds.Utilities;
using Microsoft.Win32;

namespace Diamonds.Operation.File;

public sealed class FileManager(ApplicationModel model, INotificationManager notificationManager) : IFileManager
{
    public void QuickSave()
    {
        var saveData = GetSerializedData();

        if (model.ActiveFilePath is not null)
        {
            var quickSaveResult = FileOperations.Save(saveData, model.ActiveFilePath);

            if (quickSaveResult)
            {
                notificationManager.PostNotification(new SaveSuccessNotification(model.ActiveFilePath));
                return;
            }
        }

        var dialog = OpenSaveFileDialog(FileManagementDefaults.FileName, FileManagementDefaults.DefaultLocation);
        if (dialog.ShowDialog() != true)
            return;

        var fileName = dialog.FileName;
        var saveResult = FileOperations.Save(saveData, fileName);

        if (saveResult)
            model.ActiveFilePath = fileName;

        INotification notification = saveResult
            ? new SaveSuccessNotification(fileName)
            : new SaveFailureNotification(fileName);

        notificationManager.PostNotification(notification);
    }

    public void SaveToPng(Canvas canvas)
    {
        var dialogFileName = model.ActiveFileName ?? FileManagementDefaults.FileName;
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
        notificationManager.PostNotification(new SaveSuccessNotification(dialog.FileName));
    }

    public void Save()
    {
        var dialogFileName = model.ActiveFileName ?? FileManagementDefaults.FileName;
        var dialogDirectory = GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        var dialog = OpenSaveFileDialog(dialogFileName, dialogDirectory);
        if (dialog.ShowDialog() != true)
            return;

        var saveData = GetSerializedData();
        var filePath = dialog.FileName;
        var saveResult = FileOperations.Save(saveData, filePath);

        if (saveResult)
            model.ActiveFilePath = filePath;
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
            notificationManager.PostNotification(new LoadFailureNotification(filePath));
            return false;
        }

        model.ActiveFilePath = filePath;
        model.SizeSettings = data.Value.SizeSettings;
        model.ColorSettings = data.Value.ColorSettings;
        model.DisplaySettings = data.Value.DisplaySettings;
        model.HighlightSettings = data.Value.HighlightSettings;
        model.FrameSizeSettings = data.Value.FrameSizeSettings;
        model.FrameColorSettings = data.Value.FrameColorSettings;

        notificationManager.PostNotification(new LoadSuccessNotification(filePath));
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
        return Path.GetDirectoryName(model.ActiveFilePath);
    }

    private SerializedData GetSerializedData()
    {
        return new SerializedData(
            model.SizeSettings,
            model.ColorSettings,
            model.DisplaySettings,
            model.HighlightSettings,
            model.FrameSizeSettings,
            model.FrameColorSettings);
    }
}