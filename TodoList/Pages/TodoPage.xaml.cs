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
using static System.Net.WebRequestMethods;
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
        public event EventHandler<TaskJob>? ShowDetail;
        public event EventHandler<TaskJob>? DeleteTask;
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
            try
            {
                var keyword = SearchTextBox.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    // Nên hiển thị thông báo cho người dùng
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm", "Thông báo");
                    SearchTextBox.Focus();
                    return;
                }

                if (TasksList == null)
                {
                    MessageBox.Show("Không có dữ liệu để tìm kiếm", "Thông báo");
                    return;
                }

                var searchResults = TasksList
                    .Where(task => task?.Title != null &&
                                  task.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                TasksListItem.ItemsSource = searchResults;

                // Thông báo kết quả tìm kiếm
                if (!searchResults.Any())
                {
                    MessageBox.Show($"Không tìm thấy kết quả nào cho từ khóa: {keyword}", "Thông báo");
                    SearchTextBox.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi");
            }
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

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filter = ((ComboBoxItem)FilterComboBox.SelectedItem).Content.ToString();

            // Kiểm tra xem TasksListItem có null không
            if (TasksListItem != null)
            {
                if (filter == "All Tasks")
                {
                    TasksListItem.ItemsSource = TasksList;
                }
                else
                {
                    // Lọc danh sách công việc dựa trên trạng thái
                    TasksListItem.ItemsSource = TasksList?.Where(t => t.Status.Equals(filter, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

        }


        // Event handler for sorting selection
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra nếu TasksListItem và TasksList không phải null
            if (TasksListItem == null || TasksList == null)
            {
                return; // Nếu bất kỳ đối tượng nào là null, thì không làm gì cả
            }

            // delete last character of sortCriteria
            string sortCriteria = ((ComboBoxItem)SortComboBox.SelectedItem).Content.ToString();

            if (sortCriteria == "Due Date ↑")
            {
                // Sắp xếp theo ngày đến hạn tăng dần
                TasksListItem.ItemsSource = TasksList?.OrderBy(t => t.DueDate).ToList();
            }
            else if (sortCriteria == "Due Date ↓")
            {
                // Sắp xếp theo ngày đến hạn giảm dần
                TasksListItem.ItemsSource = TasksList?.OrderByDescending(t => t.DueDate).ToList();
            }
            else if (sortCriteria == "Priority ↑")
            {
                // Sắp xếp theo độ ưu tiên tăng dần (Low -> Medium -> High)
                TasksListItem.ItemsSource = TasksList?.OrderBy(t => GetPriorityRank(t.Priority)).ToList();
            }
            else if (sortCriteria == "Priority ↓")
            {
                // Sắp xếp theo độ ưu tiên giảm dần (High -> Medium -> Low)
                TasksListItem.ItemsSource = TasksList?.OrderByDescending(t => GetPriorityRank(t.Priority)).ToList();
            }
        }

        // Hàm để ánh xạ độ ưu tiên thành thứ tự số học
        private int GetPriorityRank(string priority)
        {
            switch (priority)
            {
                case "Low":
                    return 1;
                case "Medium":
                    return 2;
                case "High":
                    return 3;
                default:
                    return 0; // Default case if priority is not recognized
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskJob)button.DataContext;
            if (task != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete this task?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    // Raise the DeleteTask event
                    DeleteTask?.Invoke(this, task);

                }
            }
        }
    }
}