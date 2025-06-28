using System.IO;

namespace Diamonds.Notification;

public readonly struct LoadSuccessNotification(string filePath) : INotification
{
    public string Message { get; } = $"Loaded {Path.GetFileName(filePath)}";
}