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
    /// Interaction logic for AddTask.xaml
    /// </summary>
    public partial class AddTask : UserControl
    {
        public List<Category>? Categories { get; set; }
        public TaskJob? Job { get; set; }

        public AddTask()
        {
            InitializeComponent();
        }
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid()) return;

            TaskJob job = new TaskJob
            {
                Title = TaskTitleTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                Status = GetSelectedStatus(),
                DueDate = GetSelectedDateTime(),
                Priority = GetSelectedPriority(),
                CreatedDate = DateTime.Now,
                CategoryId = int.TryParse(CategoryComboBox.SelectedValue?.ToString(), out int categoryId) ? categoryId : (int?)null
            };

            Job = job;
            DialogHost.CloseDialogCommand.Execute(null, null);
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
            if (!DueDateDatePicker.SelectedDate.HasValue || !DueTimeTimePicker.SelectedTime.HasValue)
                return null;

            var selectedDate = DueDateDatePicker.SelectedDate.Value;
            var selectedTime = DueTimeTimePicker.SelectedTime.Value;
            return selectedDate.Date.Add(selectedTime.TimeOfDay);
        }

        private bool IsValid()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            {
                TaskTitleErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                TaskTitleErrorText.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
            {
                TaskNameErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                TaskNameErrorText.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                DescriptionErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                DescriptionErrorText.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }

        private DateTime GetDueDateTime()
        {
            DateTime dueDate = DueDateDatePicker.SelectedDate ?? DateTime.Today;
            TimeSpan dueTime = DueTimeTimePicker.SelectedTime?.TimeOfDay ?? TimeSpan.Zero;
            return dueDate.Date + dueTime;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            if (Categories != null)
            {
                CategoryComboBox.ItemsSource = Categories;
                CategoryComboBox.SelectedValuePath = "Id";
                CategoryComboBox.DisplayMemberPath = "Type";
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
