using System.IO;

namespace DiamondLab.Operation.File;

public static class FileManagementDefaults
{
    public const string FileName = "diamonds";

    public static string DefaultLocation => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Diamonds");

    public const string FileExtension = "DMNDS";
}