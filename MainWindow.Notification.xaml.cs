using System.Windows.Threading;
using DiamondLab.Operation.Notification;

namespace DiamondLab;

public partial class MainWindow
{
    private readonly DispatcherTimer _notificationTimer = new();
    private readonly TimeSpan _notificationDuration = TimeSpan.FromSeconds(3);

    private void InitializeNotificationTimer()
    {
        _notificationTimer.Interval = _notificationDuration;
        _notificationTimer.Tick += (_, _) =>
        {
            NotificationBox.Content = "";
            _notificationTimer.Stop();
        };
    }

    private void PostNotification(INotification notification)
    {
        NotificationBox.Content = notification.Message;

        _notificationTimer.Stop();
        _notificationTimer.Start();
    }
}