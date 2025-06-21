using System.Windows;

namespace Diamonds;

public partial class App
{
    protected override void OnStartup(StartupEventArgs startupArgs)
    {
        string? startupPath = null;
        if (startupArgs.Args.Length > 0)
            startupPath = startupArgs.Args[0];

        var mainWindow = new MainWindow(startupPath);
        mainWindow.Show();

        base.OnStartup(startupArgs);
    }
}