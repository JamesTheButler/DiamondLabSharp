using System.IO;

namespace Diamonds.Utilities;

public static class FileManagerExtensions
{
    public static string? GetActiveFileName(this FileManager manager) => Path.GetFileNameWithoutExtension(manager.ActiveFilePath);
    public static string? GetActiveFileLocation(this FileManager manager) => Path.GetDirectoryName(manager.ActiveFilePath);
}