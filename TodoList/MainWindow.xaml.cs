﻿using MaterialDesignColors;
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
            todoPage.MarkDone += (s, task) =>
            {
                _taskService.UpdateTaskJob(task);
                LoadPage(_currentTaskType);
            };

            todoPage.Search += TodoPage_SearchHandler;

            await Task.Run(() =>
            {
                todoPage.TasksList = _taskService.GetTasks(type);
                _currentTaskType = type;
            });

            FrameTodo.Navigate(todoPage);
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
            FrameTodo.Navigate(categoryPage);
            SetButton(CategoryBtn);
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

            LoadPage(_currentTaskType);
        }
    }
}