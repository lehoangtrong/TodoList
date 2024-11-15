using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Todolist.DAL.Entities;

namespace Todolist.DAL
{
    public partial class TodoApplicationPrn212Context : DbContext
    {
        private readonly string _jsonFilePath;
        private TodoData _dataStore;
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<TaskJob> TaskJobs { get; set; }

        public TodoApplicationPrn212Context()
        {
            _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
            LoadJsonData();
        }

        public TodoApplicationPrn212Context(DbContextOptions<TodoApplicationPrn212Context> options)
            : base(options)
        {
            _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
            LoadJsonData();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TodoListDb");
            }
        }

        private void LoadJsonData()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(_jsonFilePath);
                    _dataStore = JsonConvert.DeserializeObject<TodoData>(jsonContent)
                        ?? new TodoData();

                    // Thêm dữ liệu vào DbSet thay vì gán
                    Categories.AddRange(_dataStore.Categories);
                    TaskJobs.AddRange(_dataStore.TaskJobs);
                    SaveChanges();
                }
                else
                {
                    _dataStore = new TodoData();
                    SaveJsonData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON data: {ex.Message}");
                _dataStore = new TodoData();
            }
        }

        private void SaveJsonData()
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(_dataStore, Formatting.Indented);
                File.WriteAllText(_jsonFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON data: {ex.Message}");
            }
        }

        public override int SaveChanges()
        {
            try
            {
                // Update the JSON data store with current entity states
                _dataStore.Categories = Categories.Local.ToList();
                _dataStore.TaskJobs = TaskJobs.Local.ToList();

                // Save to JSON file
                SaveJsonData();

                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveChanges: {ex.Message}");
                throw;
            }
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}