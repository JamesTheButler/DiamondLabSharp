using System.IO;

namespace DiamondLab.Operation.Notification;

public readonly struct SaveFailureNotification(string filePath) : INotification
{
    public string Message { get; } = $"Failed to save {Path.GetFileName(filePath)}";
}