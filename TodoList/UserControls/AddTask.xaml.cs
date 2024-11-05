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
            TaskJob job = new TaskJob();
            job.Title = TitleNameTextBox.Text;
            job.Description = DescriptionTextBox.Text;

            job.Status = StatusButton.SelectedValue?.ToString();
            job.DueDate = DateTime.Parse(DueDateDatePicker.ToString());
            job.Priority = PriorityComboBox.Text;
            job.CreatedDate = DateTime.Now;

            if (int.TryParse(CategoryComboBox.SelectedValue?.ToString(), out int categoryId))
            {
                job.CategoryId = categoryId;
            }
            else
            {
                job.CategoryId = null;
            }

            Job = job;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();
            CategoryComboBox.ItemsSource = Categories;
            CategoryComboBox.SelectedIndex = 0;
            CategoryComboBox.SelectedValuePath = "Id";
            CategoryComboBox.DisplayMemberPath = "Type";
        }
    }
}
