using DiamondLab.Model;
using DiamondLab.Operation.File;
using DiamondLab.Operation.Notification;

namespace DiamondLab.Operation;

public static class Dependencies
{
    public static ApplicationModel Model { get; } = new();
    public static INotificationManager NotificationManager { get; } = new NotificationManager();
    public static IFileManager FileManager { get; } = new FileManager(Model, NotificationManager);
}