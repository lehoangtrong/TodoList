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
using Todolist.DAL.Entities;

namespace TodoList.UserControls
{
    /// <summary>
    /// Interaction logic for DetailTask.xaml
    /// </summary>
    public partial class DetailTask : UserControl
    {
        public List<Category> Categories { get; set; }
        public TaskJob TaskJob { get; set; }
        public event EventHandler<TaskJob> UpdateTask;
        public DetailTask()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TaskJob == null || Categories == null)
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    return;
                }

                InitializeControls();
                LoadTaskJobData();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading task details: {ex.Message}");
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
        }

        private void InitializeControls()
        {
            CategoryComboBox.ItemsSource = Categories;
            CategoryComboBox.DisplayMemberPath = "Type";
            CategoryComboBox.SelectedValuePath = "Id";
        }

        private void LoadTaskJobData()
        {
            TitleTextBox.Text = TaskJob.Title;
            DescriptionTextBox.Text = TaskJob.Description;
            StatusComboBox.SelectedIndex = TaskJob.Status switch
            {
                "Pending" => 0,
                "Completed" => 1,
                _ => 0
            };
            CategoryComboBox.SelectedValue = TaskJob.CategoryId;
            CreatedDateTextBlock.Text = TaskJob.CreatedDate?.ToString("dd/MM/yyyy hh:mm tt");

            LoadDueDateAndTime();
            LoadPriority();
        }

        private void LoadDueDateAndTime()
        {
            if (TaskJob.DueDate.HasValue)
            {
                DueTimeDate.SelectedDate = TaskJob.DueDate.Value.Date;
                DueTimePicker.SelectedTime = TaskJob.DueDate.Value;
            }
        }

        private void LoadPriority()
        {
            PriorityComboBox.SelectedIndex = TaskJob.Priority switch
            {
                "High" => 0,
                "Medium" => 1,
                "Low" => 2,
                _ => 0
            };
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var updatedTask = CreateUpdatedTask();
            UpdateTask?.Invoke(this, updatedTask);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private TaskJob CreateUpdatedTask()
        {
            return new TaskJob
            {
                Id = TaskJob.Id,
                Title = TitleTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                Status = GetSelectedStatus(),
                DueDate = GetSelectedDateTime(),
                Priority = GetSelectedPriority(),
                CategoryId = (int)CategoryComboBox.SelectedValue,
                CreatedDate = TaskJob.CreatedDate
            };
        }

        private string GetSelectedStatus()
        {
            var selectedItem = (ComboBoxItem)StatusComboBox.SelectedItem;
            var stackPanel = (StackPanel)selectedItem.Content;
            return ((TextBlock)stackPanel.Children[1]).Text;
        }

        private string GetSelectedPriority()
        {
            var priorityItem = (ComboBoxItem)PriorityComboBox.SelectedItem;
            var priorityStack = (StackPanel)priorityItem.Content;
            return ((TextBlock)priorityStack.Children[1]).Text;
        }

        private DateTime? GetSelectedDateTime()
        {
            if (!DueTimeDate.SelectedDate.HasValue || !DueTimePicker.SelectedTime.HasValue)
                return null;

            var selectedDate = DueTimeDate.SelectedDate.Value;
            var selectedTime = DueTimePicker.SelectedTime.Value;
            return selectedDate.Date.Add(selectedTime.TimeOfDay);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ShowError("Title is required");
                return false;
            }

            if (!DueTimeDate.SelectedDate.HasValue || !DueTimePicker.SelectedTime.HasValue)
            {
                ShowError("Due date and time are required");
                return false;
            }

            var selectedDateTime = GetSelectedDateTime();
            if (selectedDateTime < DateTime.Now)
            {
                ShowError("Due date cannot be in the past");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
