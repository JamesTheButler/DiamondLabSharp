namespace Diamonds.Utilities;

public class FileManager
{
    private static FileManager? _instance;
    public static FileManager Instance => _instance ??= new FileManager();
    
    /// <summary>
    /// Complete path to the file that is being edited
    /// </summary>
    public string? ActiveFilePath { get; set; } 
}