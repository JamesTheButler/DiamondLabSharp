using Diamonds.Model;
using Diamonds.Operation.File;
using Diamonds.Operation.Notification;

namespace Diamonds.Operation;

public static class Dependencies
{
    public static ApplicationModel Model { get; } = new();
    public static INotificationManager NotificationManager { get; } = new NotificationManager();
    public static IFileManager FileManager { get; } = new FileManager(Model, NotificationManager);
}