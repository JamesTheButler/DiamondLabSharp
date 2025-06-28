using System.IO;

namespace Diamonds.Notification;

public readonly struct LoadFailureNotification(string filePath) : INotification
{
    public string Message { get; } = $"Failed to load {Path.GetFileName(filePath)}";
}