using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;

namespace TodoList
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        private MainWindow mainWindow;

        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            mainWindow = new MainWindow();
            mainWindow.Show();

            // Load tray icon
            _notifyIcon.Icon = new System.Drawing.Icon("Resources/iconv2.ico");
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Todo App";


            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Show", null, (s, ea) =>
            {
                mainWindow.Show(); // Use Show() instead of WindowState.Normal if the window might have been closed
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            });

            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, ea) =>
            {
                Shutdown(); // Properly shut down the application
            });

            mainWindow.Closing += (s, ea) =>  // Minimize to tray instead of closing
            {
                ea.Cancel = true;            // Prevent the window from actually closing
                mainWindow.Hide();           // Hide the window
            };


            base.OnStartup(e);

        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose();

            base.OnExit(e);
        }

    }

}
