namespace Diamonds.Operation.Notification;

public interface INotificationManager
{
    public event Action<INotification>? NotificationPosted;

    void PostNotification(INotification notification);
}