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

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            // add material design button click event
            var task = (TaskJob)((Button)sender).DataContext;
            if (task == null)
            {
                MessageBox.Show("Task not found");
                return;
            }

            Button button = (Button)sender;
            // add material design button progress assist
            ButtonProgressAssist.SetIsIndeterminate(button, true);
            ButtonProgressAssist.SetIsIndicatorVisible(button, true);

            // task status change
            task.Status = "Completed";

            // Invoke the external event handler
            MarkDone?.Invoke(this, task);
        }
    }
}
