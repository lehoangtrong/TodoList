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
using TodoList.Windows;

namespace TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CategoryService _categoryService = new CategoryService();
        private TaskService _taskService = new TaskService();
        public bool IsDialogOpen { get; set; }
        private TaskService.TaskType _currentTaskType;
        private readonly DialogHost _dialogs;
        public MainWindow()
        {
            InitializeComponent();
            FrameTodo.Navigate(null);

            _dialogs = new DialogHost();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetButton(TodayBtn);
            LoadPage(TaskService.TaskType.Today);
        }

        private async void LoadPage(TaskService.TaskType type)
        {
            ShowLoadingPage();
            string title = type switch
            {
                TaskService.TaskType.Today => "Today Tasks!!",
                TaskService.TaskType.Upcoming => "Upcoming Tasks!!",
                TaskService.TaskType.Important => "Important Tasks!!",
                TaskService.TaskType.Completed => "Completed Tasks!!",
                TaskService.TaskType.Planned => "Planned Tasks!!",
                TaskService.TaskType.All => "All Tasks!!",
                _ => "Tasks"
            };

            TodoPage todoPage = new TodoPage();
            todoPage.TodoTextBlock.Text = title;
            todoPage.MarkDone += TodoPage_MarkDone;

            todoPage.Search += TodoPage_SearchHandler;
            todoPage.ShowDetail += TodoPage_ShowDetail;

            await Task.Run(() =>
            {
                todoPage.TasksList = _taskService.GetTasks(type);
                _currentTaskType = type;
            });

            FrameTodo.Navigate(todoPage);
        }

        private void TodoPage_MarkDone(object? sender, TaskJob e)
        {
            _taskService.UpdateTaskJob(e);
            LoadPage(_currentTaskType);
        }

        private void TodoPage_ShowDetail(object? sender, TaskJob e)
        {
            DetailTask detailTask = new();
            detailTask.Categories = _categoryService.GetAllCategorys();
            detailTask.UpdateTask += DetailTask_UpdateTask;
            detailTask.TaskJob = e;
            _ = DialogHost.Show(detailTask, "DialogHostMain");
        }

        private void DetailTask_UpdateTask(object? sender, TaskJob e)
        {
            _taskService.UpdateTaskJob(e);
            LoadPage(_currentTaskType);
        }

        private void TodoPage_SearchHandler(object? sender, string e)
        {
            if (e == null)
            {
                LoadPage(_currentTaskType);
                return;
            }

            TodoPage todoPage = (TodoPage)sender;
            todoPage.TasksList = _taskService.Search(e);
            FrameTodo.Navigate(todoPage);
        }

        private void SetButton(Button button)
        {
            SetButtonHighlight(TodayBtn, button == TodayBtn);
            SetButtonHighlight(UpcomingBtn, button == UpcomingBtn);
            SetButtonHighlight(ImportantBtn, button == ImportantBtn);
            SetButtonHighlight(CompletedBtn, button == CompletedBtn);
            SetButtonHighlight(PlannedBtn, button == PlannedBtn);
            SetButtonHighlight(AllBtn, button == AllBtn);
            SetButtonHighlight(CategoryBtn, button == CategoryBtn);
            SetButtonHighlight(QuitBtn, button == QuitBtn);
        }

        private void SetButtonHighlight(Button button, bool isHighlighted)
        {
            if (isHighlighted)
            {
                button.Style = (Style)FindResource("MaterialDesignRaisedButton");
            }
            else
            {
                button.Style = (Style)FindResource("MaterialDesignFlatButton");
            }
        }

        private void ShowLoadingPage()
        {
            FrameTodo.Navigate(new LoadingPage());
        }

        private void TodayBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(TaskService.TaskType.Today);
            SetButton(TodayBtn);
        }

        private void UpcomingBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(TaskService.TaskType.Upcoming);
            SetButton(UpcomingBtn);
        }

        private void ImportantBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(TaskService.TaskType.Important);
            SetButton(ImportantBtn);
        }

        private void CompletedBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(TaskService.TaskType.Completed);
            SetButton(CompletedBtn);
        }

        private void PlannedBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(TaskService.TaskType.Planned);
            SetButton(PlannedBtn);
        }

        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(TaskService.TaskType.All);
            SetButton(AllBtn);
        }

        private void CategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            CategoryPage categoryPage = new();
            categoryPage.AddCategory += CategoryPage_AddCategory;
            categoryPage.UpdateCategory += CategoryPage_UpdateCategory;
            categoryPage.DeleteCategory += CategoryPage_DeleteCategory;
            categoryPage.Category = _categoryService.GetAllCategorys();
            FrameTodo.Navigate(categoryPage);
            SetButton(CategoryBtn);
        }
        private void LoadCategoryPage(object s, Category e)
        {
            CategoryPage categoryPage = new();
            categoryPage.Category = _categoryService.GetAllCategorys();
            categoryPage.AddCategory += CategoryPage_AddCategory;
            categoryPage.UpdateCategory += CategoryPage_UpdateCategory;
            categoryPage.DeleteCategory += CategoryPage_DeleteCategory;
            FrameTodo.Navigate(categoryPage);
        }

        private void CategoryPage_DeleteCategory(object? sender, Category e)
        {
            _categoryService.RemoveCategory(e);
            LoadCategoryPage(sender, e);
        }

        private void CategoryPage_AddCategory(object? sender, EventArgs e)
        {
            CategoryUserControl addCategory = new();
            addCategory.AddCategory += (s, e) =>
            {
                _categoryService.AddCategory(e);
                LoadCategoryPage(s, e);
            };
            _ = DialogHost.Show(addCategory, "DialogHostMain");
        }

        private void CategoryPage_UpdateCategory(object? sender, Category e)
        {
            CategoryUserControl updateCategory = new();
            updateCategory.UpdateCategory += (s, e) =>
            {
                _categoryService.UpdateCategory(e);
                LoadCategoryPage(s, e);
            };
            updateCategory.EditedOne = e;
            _ = DialogHost.Show(updateCategory, "DialogHostMain");
        }

        private async void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTask addTask = new();
            addTask.Categories = _categoryService.GetAllCategorys();
            _ = await DialogHost.Show(addTask, "DialogHostMain");
            TaskJob newTask = addTask.Job;

            if (newTask == null) return;
            // save to database
            // loading screen while saving
            _taskService.AddTaskJob(newTask);
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            var result = MessageBox.Show("Are you sure you want to quit?", "Confirm Quit", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Nếu người dùng chọn Yes, thoát ứng dụng
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            // Nếu người dùng chọn No, không làm gì cả và ứng dụng vẫn hoạt động
            else
            {
                // Thông báo rằng họ đã chọn ở lại ứng dụng
                MessageBox.Show("You choose to stay in the app", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            PaletteHelper palette = new PaletteHelper();

            ITheme theme = palette.GetTheme();

            if (DarkModeToggle.IsChecked.Value)
            {
                theme.SetBaseTheme(Theme.Dark);
            }
            else
            {
                theme.SetBaseTheme(Theme.Light);
            }
            palette.SetTheme(theme);
        }

        private void DarkModeToggle_Click(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            if (DarkModeToggle.IsChecked == true)
            {
                theme.SetBaseTheme(Theme.Dark);
                DarkModeToggle.Content = new PackIcon { Kind = PackIconKind.WeatherNight };
            }
            else
            {
                theme.SetBaseTheme(Theme.Light);
                DarkModeToggle.Content = new PackIcon { Kind = PackIconKind.WeatherSunny };
            }

            paletteHelper.SetTheme(theme);

        }
    }
}