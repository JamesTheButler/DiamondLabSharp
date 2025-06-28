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
            FileOperations.Save(saveData, _fileManager.ActiveFilePath);
            return;
        }

        var dialog = OpenSaveFileDialog(FileManagementDefaults.FileName, FileManagementDefaults.DefaultLocation);
        if (dialog.ShowDialog() != true)
            return;

        
        var isSaveSuccess = FileOperations.Save(saveData, dialog.FileName);

        if (isSaveSuccess)
            _fileManager.ActiveFilePath = dialog.FileName;
    }

    private SerializedData GetSerializedData()
    {
        return new SerializedData(_currentSizeSettings, _currentColorSettings, _currentDisplaySettings);
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
        var isSaveSuccess = FileOperations.Save(saveData, dialog.FileName);

        if (isSaveSuccess)
            _fileManager.ActiveFilePath = dialog.FileName;
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

        LoadFile(dialog.FileName);

        ReDraw();
    }

    private void LoadFile(string filePath)
    {
        var data = FileOperations.Load(filePath);
        if (data is null)
        {
            Console.WriteLine($"Failed to load or parse '{filePath}'");
            return;
        }

        _fileManager.ActiveFilePath = filePath;
        _currentSizeSettings = data.Value.SizeSettings;
        _currentColorSettings = data.Value.ColorSettings;
        _currentDisplaySettings = data.Value.DisplaySettings;
        
        UnbindInputs();
        RefreshColorInputs();
        RefreshDimensionInputs();
        RefreshDisplayInputs();
        BindInputs();
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
    }
}