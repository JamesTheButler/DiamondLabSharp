using System.IO;

namespace DiamondLab.Operation.Notification;

public readonly struct SaveSuccessNotification(string filePath) : INotification
{
    public string Message { get; } = $"Saved {Path.GetFileName(filePath)}";
}