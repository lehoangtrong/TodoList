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
using Todolist.BLL.Services;
using Todolist.DAL.Entities;

namespace TodoList.UserControls
{
    /// <summary>
    /// Interaction logic for CategoryUserControl.xaml
    /// </summary>
    public partial class CategoryUserControl : UserControl
    {
        private CategoryService _categoryService = new();
        public Category EditedOne { get; set; }
        public CategoryUserControl()
        {
            InitializeComponent();
        }

        private void SaveCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Category obj = new();
           
            obj.Type = TypeTextBox.Text;
            obj.Description = DescriptionTextBox.Text;
            if (EditedOne == null) 
            {
                _categoryService.AddCategory(obj);

            }
            else
            {
                obj.Id = EditedOne.Id;
                obj.CreatedDate = EditedOne.CreatedDate;
                _categoryService.UpdateCategory(obj);
            }

            
            DialogHost.CloseDialogCommand.Execute(null, this);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (EditedOne == null)
            {
                TitleWindowModeTextBlock.Text = "Add new category";
                return;
            }
            TitleWindowModeTextBlock.Text = "Update current category";
            TypeTextBox.Text = EditedOne.Type;
            DescriptionTextBox.Text = EditedOne.Description;

        }
    }
}
