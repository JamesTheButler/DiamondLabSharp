using System.Windows.Controls;

namespace DiamondLab.Operation.File;

public interface IFileManager
{
    bool Load();
    bool Load(string path);

    void Save();
    void QuickSave();
    void SaveToPng(Canvas canvas);
}