﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Todolist.BLL.Services;
using Todolist.DAL.Entities;
using TodoList.UserControls;

namespace TodoList.Windows
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        private CategoryService _categoryService = new();
        private List<Category> _categories;
        private int _currentPage = 0;  // Current page index
        private const int _itemsPerPage = 10;  // Items per page

        public CategoryWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
            UpdatePageNumber();
        }

        private void LoadCategories()
        {
            _categories = _categoryService.GetAllCategorys();
            DisplayCurrentPage();
        }

        private void DisplayCurrentPage()
        {
            if (_categories != null && _categories.Count > 0)
            {
                int totalItems = _categories.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / _itemsPerPage);

                // Get the items for the current page
                var pagedData = _categories.Skip(_currentPage * _itemsPerPage).Take(_itemsPerPage).ToList();
                CategoryDataGrid.ItemsSource = pagedData;
            }
            else
            {
                CategoryDataGrid.ItemsSource = null; // Clear data grid if no categories are found
            }
        }

        private void UpdatePageNumber()
        {
            PageNumberTextBlock.Text = $"Page {_currentPage + 1}";
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var categoryUserControl = new CategoryUserControl();
            // Show the UserControl in the DialogHost
            await DialogHost.Show(categoryUserControl, "RootDialog");

            // Reload categories and refresh the DataGrid
            LoadCategories();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                DisplayCurrentPage();
                UpdatePageNumber();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)_categories.Count / _itemsPerPage);
            if (_currentPage < totalPages - 1)
            {
                _currentPage++;
                DisplayCurrentPage();
                UpdatePageNumber();
            }
        }
    }
}