using System.IO;

namespace DiamondLab.Operation.Notification;

public readonly struct LoadFailureNotification(string filePath) : INotification
{
    public string Message { get; } = $"Failed to load {Path.GetFileName(filePath)}";
}