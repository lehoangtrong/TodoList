using MaterialDesignThemes.Wpf;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public CategoryService CategoryService { get; set; }
        public Category EditedOne { get; set; }
        public CategoryUserControl()
        {
            InitializeComponent();
        }

        private void SaveCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }
            Category obj = new();

            obj.Type = TypeTextBox.Text;
            obj.Description = DescriptionTextBox.Text;
            if (EditedOne == null) 
            {
                CategoryService.AddCategory(obj);

            }
            else
            {
                obj.Id = EditedOne.Id;
                obj.CreatedDate = EditedOne.CreatedDate;
                CategoryService.UpdateCategory(obj);
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
        private bool ValidateInput()
        {
            bool isValid = true;

            string categoryType = TypeTextBox.Text?.Trim().ToLower();
            string description = DescriptionTextBox.Text?.Trim();

            // Validate Category Type
            if (string.IsNullOrEmpty(categoryType))
            {
                TypeErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (categoryType.Length < 5 || categoryType.Length > 80) 
            { 
                TypeErrorText.Visibility= Visibility.Visible;
                TypeErrorText.Text = "Category type must be between 5-80 chars long";
                isValid = false;
            }else
            {
                TypeErrorText.Visibility = Visibility.Collapsed;
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                TypeTextBox.Text = textInfo.ToTitleCase(categoryType);
            }

            // Validate Description
            if (string.IsNullOrEmpty(description))
            {
                DescriptionErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (description.Length < 5 || description.Length > 100)
            {
                DescriptionErrorText.Visibility= Visibility.Visible;
                DescriptionErrorText.Text = "Description must be between 5-200 chars long";
                isValid= false;
            }else
            {
                DescriptionErrorText.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }
    }
}
