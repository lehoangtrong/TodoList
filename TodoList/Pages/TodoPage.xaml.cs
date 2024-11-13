using MaterialDesignThemes.Wpf;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Todolist.BLL.Services;
using Todolist.DAL.Entities;
using TodoList.UserControls;
using static Todolist.BLL.Services.TaskService;

namespace TodoList.Pages
{
    /// <summary>
    /// Interaction logic for TodoPage.xaml
    /// </summary>
    public partial class TodoPage : Page
    {
        public List<TaskJob>? TasksList { get; set; }
        // Define the event handler delegate
        public event EventHandler<TaskJob>? MarkDone;
        public event EventHandler<string>? Search;
        public event EventHandler<TaskJob>? ShowDetail;
        private IWavePlayer waveOut;
        private Mp3FileReader mp3Reader;
        public TodoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // show tasks in list
            if (TasksList != null)
            {
                TasksListItem.ItemsSource = TasksList;
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // return search all tasks
                Search?.Invoke(this, "all");
                return;
            }

            // Invoke the external event handler
            Search?.Invoke(this, keyword);

            RefreshPage();
        }

        private void RefreshPage()
        {
            TasksListItem.ItemsSource = null;
            TasksListItem.ItemsSource = TasksList;
        }

        private void UpdateTaskStatus(object sender, string newStatus)
        {
            var checkBox = (CheckBox)sender;
            var task = (TaskJob)checkBox.DataContext;
            if (task != null)
            {
                task.Status = newStatus;
                MarkDone?.Invoke(this, task);
            }
        }

        private void PlayCompletionSound()
        {
            try
            {
                // Lấy tài nguyên nhúng MP3 từ ứng dụng và chuyển thành MemoryStream
                var resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/TodoList;component/Resources/finish.mp3")).Stream;
                var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                // Khởi tạo NAudio để phát âm thanh từ MemoryStream
                mp3Reader = new Mp3FileReader(memoryStream);
                waveOut = new WaveOutEvent();
                waveOut.Init(mp3Reader);
                waveOut.Play();

                // Đăng ký sự kiện để dừng và giải phóng tài nguyên sau khi phát xong
                waveOut.PlaybackStopped += (s, e) =>
                {
                    waveOut.Dispose();
                    mp3Reader.Dispose();
                    memoryStream.Dispose();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is CheckBox)
            {
                return; // Bỏ qua nếu click vào CheckBox
            }

            if (sender is MaterialDesignThemes.Wpf.Card card && card.DataContext is TaskJob task)
            {
                ShowDetail?.Invoke(this, task);
            }
        }

        private void DoneCheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string initStatus = checkBox.IsChecked == true ? "Completed" : "Pending";
            if (initStatus != "Completed")
            {
                PlayCompletionSound();
            }
            // Cập nhật binding expression để giao diện phản ánh thay đổi
            var bindingExpression = BindingOperations.GetBindingExpression(checkBox, CheckBox.IsCheckedProperty);
            bindingExpression?.UpdateTarget();
            // Force the UI to render immediately
            Dispatcher.Invoke(() => { });

            // Delay 5 giây trước khi cập nhật task status
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(5000);

                string currentStatus = checkBox.IsChecked == true ? "Completed" : "Pending";

                if (initStatus != currentStatus)
                {
                    UpdateTaskStatus(sender, currentStatus);
                }
            });
        }
        public void SetTaskType(TaskService.TaskType taskType)
        {
            CurrentTaskType = taskType;
        }
    }
}
