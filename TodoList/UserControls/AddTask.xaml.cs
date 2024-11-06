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
                Title = TaskTitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                Status = (StatusButton.SelectedItem as ComboBoxItem)?.Content.ToString(),
                DueDate = DueDateDatePicker.SelectedDate ?? DateTime.Now,
                Priority = PriorityComboBox.Text,
                CreatedDate = DateTime.Now,
                CategoryId = int.TryParse(CategoryComboBox.SelectedValue?.ToString(), out int categoryId) ? categoryId : (int?)null
            };

            Job = job;

            // close dialog
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private bool IsValid()
        {
            bool isValid = true;

            // Check TaskTitleTextBox
            bool hasError = string.IsNullOrWhiteSpace(TaskTitleTextBox.Text);
            SetValidationError(TaskTitleTextBox, TaskTitleErrorText, hasError, "Task Title is required.");
            if (hasError) isValid = false;

            // Check TaskNameTextBox
            hasError = string.IsNullOrWhiteSpace(TaskNameTextBox.Text);
            SetValidationError(TaskNameTextBox, TaskNameErrorText, hasError, "Task Name is required.");
            if (hasError) isValid = false;

            // Check DescriptionTextBox
            hasError = string.IsNullOrWhiteSpace(DescriptionTextBox.Text);
            SetValidationError(DescriptionTextBox, DescriptionErrorText, hasError, "Description is required.");
            if (hasError) isValid = false;

            // Check DueDateDatePicker
            hasError = !DueDateDatePicker.SelectedDate.HasValue;
            SetValidationError(DueDateDatePicker, DueDateErrorText, hasError, "Due Date is required.");
            if (hasError) isValid = false;

            // Check PriorityComboBox
            hasError = PriorityComboBox.SelectedItem == null;
            SetValidationError(PriorityComboBox, null, hasError, "Priority is required.");
            if (hasError) isValid = false;

            // Check CategoryComboBox
            hasError = CategoryComboBox.SelectedItem == null;
            SetValidationError(CategoryComboBox, null, hasError, "Category is required.");
            if (hasError) isValid = false;

            // Check StatusButton
            hasError = StatusButton.SelectedItem == null;
            SetValidationError(StatusButton, null, hasError, "Status is required.");
            if (hasError) isValid = false;

            return isValid;
        }


        private void SetValidationError(Control control, TextBlock errorTextBlock, bool hasError, string errorMessage)
        {
            if (hasError)
            {
                if (errorTextBlock != null)
                    errorTextBlock.Visibility = Visibility.Visible;

                control.ToolTip = errorMessage;
                control.BorderBrush = Brushes.Red;
            }
            else
            {
                if (errorTextBlock != null)
                    errorTextBlock.Visibility = Visibility.Collapsed;

                control.ToolTip = null;
                control.ClearValue(BorderBrushProperty);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            CategoryComboBox.ItemsSource = Categories;
            CategoryComboBox.SelectedValuePath = "Id";
            CategoryComboBox.DisplayMemberPath = "Type";
            CategoryComboBox.SelectedIndex = 0;
        }
    }
}
