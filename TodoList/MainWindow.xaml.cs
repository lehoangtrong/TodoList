using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Todolist.BLL.Services;
using Todolist.DAL;
using Todolist.DAL.Entities;
using TodoList.Pages;
using TodoList.UserControls;

namespace TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CategoryService _categoryService = new CategoryService();
        public bool IsDialogOpen { get; set; }

        private readonly DialogHost _dialogs;
        public MainWindow()
        {
            InitializeComponent();
            FrameTodo.Navigate(null);

            _dialogs = new DialogHost();
        }

        private void OnShowDialog()
        {
        }

        private void AddTaskControl_TaskAdded(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Category> categories = _categoryService.GetAllCategorys();
            IsDialogOpen = true;

            AddTask addTask = new()
            {
                Categories = categories
            };
            _ = await _dialogs.ShowDialog(addTask);
        }

        private void TodayBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate(new TodayTasksPage());
        }

        private void UpcommingBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate("Upcomming Page");
        }

        private void ImportantBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate("Important Page");

        }

        private void CompletedBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate("Complete Page");
        }

        private void PlannedBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate("Planned Page");
        }

        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate("All Page");
        }

        private void OverdueBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameTodo.Navigate("Overdue Page");
        }

        private async void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTask addTask = new();
            addTask.Categories = _categoryService.GetAllCategorys();
            await DialogHost.Show(addTask, "DialogHostMain");
        }
    }
}
