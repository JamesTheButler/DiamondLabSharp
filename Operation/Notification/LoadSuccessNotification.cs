using System.IO;

namespace DiamondLab.Operation.Notification;

public readonly struct LoadSuccessNotification(string filePath) : INotification
{
    public string Message { get; } = $"Loaded {Path.GetFileName(filePath)}";
}