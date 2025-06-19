using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diamonds.Utilities;

public static class FileOperations
{
    public static void SaveCanvasToPng(Canvas canvas, string filePath)
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
}