using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diamonds.Utilities;

public static class FileOperations
{
    public static void SaveAsPng(Canvas canvas, string filePath)
    {
        var size = new Size(canvas.ActualWidth, canvas.ActualHeight);
        canvas.Measure(size);
        canvas.Arrange(new Rect(size));

        var bitmap = new RenderTargetBitmap(
            (int)size.Width, (int)size.Height,
            96, 96, PixelFormats.Pbgra32);
        bitmap.Render(canvas);

        var pngEncoder = new PngBitmapEncoder();
        pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var fileStream = new FileStream(filePath, FileMode.Create);
        pngEncoder.Save(fileStream);
    }

    public static bool Save(FileFormat data, string filePath)
    {
        if (!filePath.EndsWith(".json"))
        {
            filePath += ".json";
        }

        try
        {
            var serializedData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, serializedData);
            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception while saving file: {exception}");
            return false;
        }
    }
    
    public static FileFormat? Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File '{filePath}' doesn't exist.");
            return null;
        }
        
        var fileContent = File.ReadAllText(filePath);
        try
        {
            return JsonSerializer.Deserialize<FileFormat>(fileContent);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception while deserializing file: {exception}");
            return null;
        }
    }
}
