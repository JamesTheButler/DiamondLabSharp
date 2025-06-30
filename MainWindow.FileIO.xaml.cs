using Diamonds.Model;
using Diamonds.Notification;
using Diamonds.Utilities;
using Microsoft.Win32;

namespace Diamonds;

public partial class MainWindow
{
    private void QuickSave()
    {
        var saveData = GetSerializedData();

        if (_fileManager.ActiveFilePath is not null)
        {
            var quickSaveResult = FileOperations.Save(saveData, _fileManager.ActiveFilePath);

            if (quickSaveResult)
            {
                PostNotification(new SaveSuccessNotification(_fileManager.ActiveFilePath));
                return;
            }
        }

        var dialog = OpenSaveFileDialog(FileManagementDefaults.FileName, FileManagementDefaults.DefaultLocation);
        if (dialog.ShowDialog() != true)
            return;

        var fileName = dialog.FileName;
        var saveResult = FileOperations.Save(saveData, fileName);

        if (saveResult)
        {
            _fileManager.ActiveFilePath = dialog.FileName;
        }

        INotification notification = saveResult
            ? new SaveSuccessNotification(fileName)
            : new SaveFailureNotification(fileName);

        PostNotification(notification);
    }

    private SerializedData GetSerializedData()
    {
        return new SerializedData(_currentSizeSettings, _currentColorSettings, _currentDisplaySettings, _highlightSettings);
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

    private void SaveToFile()
    {
        var dialogFileName = _fileManager.GetActiveFileName() ?? FileManagementDefaults.FileName;
        var dialogDirectory = _fileManager.GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        var dialog = OpenSaveFileDialog(dialogFileName, dialogDirectory);
        if (dialog.ShowDialog() != true)
            return;

        var saveData = GetSerializedData();
        var fileName = dialog.FileName;
        var saveResult = FileOperations.Save(saveData, fileName);

        if (saveResult)
        {
            _fileManager.ActiveFilePath = dialog.FileName;
        }

        INotification notification = saveResult
            ? new SaveSuccessNotification(fileName)
            : new SaveFailureNotification(fileName);

        PostNotification(notification);
    }

    private void LoadFromFile()
    {
        var dialogDirectory = _fileManager.GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        const string extension = FileManagementDefaults.FileExtension;
        var dialog = new OpenFileDialog
        {
            Filter = $"{extension} Files (*.{extension})|*.{extension}",
            DefaultExt = $".{extension}",
            DefaultDirectory = dialogDirectory
        };

        if (dialog.ShowDialog() != true)
            return;

        var fileName = dialog.FileName;
        var loadResult = LoadFile(fileName);

        INotification notification = loadResult
            ? new LoadSuccessNotification(fileName)
            : new LoadFailureNotification(fileName);

        PostNotification(notification);

        ReDraw();
    }

    private bool LoadFile(string filePath)
    {
        var data = FileOperations.Load(filePath);
        if (data is null)
        {
            Console.WriteLine($"Failed to load or parse '{filePath}'");
            return false;
        }

        _fileManager.ActiveFilePath = filePath;
        _currentSizeSettings = data.Value.SizeSettings;
        _currentColorSettings = data.Value.ColorSettings;
        _currentDisplaySettings = data.Value.DisplaySettings;
        _highlightSettings = data.Value.HighlightSettings;

        UnbindInputs();
        RefreshColorInputs();
        RefreshDimensionInputs();
        RefreshDisplayInputs();
        BindInputs();
        return true;
    }

    private void SaveToPng()
    {
        var dialogFileName = _fileManager.GetActiveFileName() ?? FileManagementDefaults.FileName;
        var dialogDirectory = _fileManager.GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        var dialog = new SaveFileDialog
        {
            Filter = "PNG Files (*.png)|*.png",
            DefaultExt = ".png",
            FileName = $"{dialogFileName}.png",
            DefaultDirectory = dialogDirectory
        };

        if (dialog.ShowDialog() != true)
            return;

        FileOperations.SaveAsPng(MainCanvas, dialog.FileName);
        PostNotification(new SaveSuccessNotification(dialog.FileName));
    }
}