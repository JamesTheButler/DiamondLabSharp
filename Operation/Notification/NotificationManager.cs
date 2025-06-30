namespace Diamonds.Operation.Notification;

public sealed class NotificationManager : INotificationManager
{
    public event Action<INotification>? NotificationPosted;

    public void PostNotification(INotification notification)
    {
        NotificationPosted?.Invoke(notification);
    }
}