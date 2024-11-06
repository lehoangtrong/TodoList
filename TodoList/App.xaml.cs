using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace TodoList
{
    public partial class App : Application
    {
        private TaskbarIcon trayIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            trayIcon = (TaskbarIcon)Application.Current.Resources["GlobalNotifyIcon"];
        }

        private void OpenApp_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Show();
                window.WindowState = WindowState.Normal;
            }
        }
        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            trayIcon.Dispose();
            Application.Current.Shutdown();  
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (trayIcon != null)
            {
                trayIcon.Dispose();
            }
            base.OnExit(e);
        }
    }
}
