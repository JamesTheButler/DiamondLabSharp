using System.IO;

namespace Diamonds.Operation.Notification;

public readonly struct SaveSuccessNotification(string filePath) : INotification
{
    public string Message { get; } = $"Saved {Path.GetFileName(filePath)}";
}