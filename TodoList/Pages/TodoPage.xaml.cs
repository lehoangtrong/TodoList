using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private MediaPlayer _mediaPlayer;
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

                // Lấy binding expression
                var bindingExpression = BindingOperations.GetBindingExpression(checkBox, CheckBox.IsCheckedProperty);

                // Cập nhật target của binding
                bindingExpression?.UpdateTarget();
            }
        }

        private void DoneCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            string newStatus = checkBox.IsChecked == true ? "Completed" : "Pending";
            if (newStatus == "Completed")
            {
                PlayCompletionSound();
            }
            // Open sound completed
            UpdateTaskStatus(sender, newStatus);
        }

        private void PlayCompletionSound()
        {
            try
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();
                }
                _mediaPlayer.Open(new Uri("https://cdn.pixabay.com/download/audio/2022/03/10/audio_2318350b97.mp3?filename=correct-choice-43861.mp3"));
                _mediaPlayer.Play();
                _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Close();
        }

        private void Card_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is MaterialDesignThemes.Wpf.Card card && card.DataContext is TaskJob task)
            {
                ShowDetail?.Invoke(this, task);
            }
        }
    }
}
