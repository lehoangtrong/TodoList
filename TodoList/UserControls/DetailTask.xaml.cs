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
            if (TaskJob == null)
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
                return;
            }

            FillComboBox(Categories);

            TitleTextBox.Text = TaskJob.Title;
            DescriptionTextBox.Text = TaskJob.Description;
            StatusTextBlock.Text = TaskJob.Status;
            DueTimeDate.SelectedDate = TaskJob.DueDate;
            PriorityComboBox.SelectedIndex = TaskJob.Priority switch
            {
                "Low" => 0,
                "Medium" => 1,
                "High" => 2,
                _ => 0
            };

            CategoryComboBox.SelectedValue = TaskJob.CategoryId;
            CreatedDateTextBlock.Text = TaskJob.CreatedDate?.ToString("dd/MM/yyyy hh:mm tt");
        }

        private void FillComboBox(List<Category> categories)
        {
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "Type";
            CategoryComboBox.SelectedValuePath = "Id";
            CategoryComboBox.SelectedIndex = 0;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid()) return;
            TaskJob updatedTask = new TaskJob();
            updatedTask.Id = TaskJob.Id;
            updatedTask.Title = TitleTextBox.Text;
            updatedTask.Description = DescriptionTextBox.Text;
            updatedTask.Status = StatusTextBlock.Text;
            updatedTask.DueDate = DueTimeDate.SelectedDate;
            updatedTask.Priority = PriorityComboBox.Text;
            updatedTask.CategoryId = (int)CategoryComboBox.SelectedValue;
            updatedTask.CreatedDate = TaskJob.CreatedDate;

            UpdateTask?.Invoke(this, updatedTask);

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Title is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (DueTimeDate.SelectedDate == null)
            {
                MessageBox.Show("Due date is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
