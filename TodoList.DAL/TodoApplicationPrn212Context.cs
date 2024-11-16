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

                    SeedDatabaseFromDataStore();
                }
                else
                {
                    _dataStore = new TodoData();
                    _dataStore.Categories = new List<Category>(); // Initialize lists to avoid null reference exceptions
                    _dataStore.TaskJobs = new List<TaskJob>();
                    SaveJsonData(); // Save the initial empty datastore
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON data: {ex.Message}");
                _dataStore = new TodoData();
            }
        }

        private void SeedDatabaseFromDataStore()
        {
            //Efficiently add only new items if the database doesn't match the data store
            var existingCategoryIds = Categories.Select(c => c.Id).ToList();
            var newCategories = _dataStore.Categories
                .Where(c => !existingCategoryIds.Contains(c.Id))
                .ToList();


            if (newCategories.Any())
            {
                Categories.AddRange(newCategories);
            }


            var existingTaskJobIds = TaskJobs.Select(t => t.Id).ToList();
            var newTaskJobs = _dataStore.TaskJobs
                .Where(t => !existingTaskJobIds.Contains(t.Id))
                .ToList();

            if (newTaskJobs.Any())
            {
                TaskJobs.AddRange(newTaskJobs);
            }

            SaveChanges();
        }


        private void SaveJsonData()
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                string jsonContent = JsonConvert.SerializeObject(_dataStore, Formatting.Indented, jsonSettings);

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
                _dataStore.Categories = Categories.Local.ToList();
                _dataStore.TaskJobs = TaskJobs.Local.ToList();

                SaveJsonData();
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveChanges: {ex.Message}");
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình cho Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd(); // ID tự tăng

                entity.Property(c => c.Type)
                    .IsRequired() // Không cho phép null
                    .HasMaxLength(100); // Giới hạn độ dài (tùy chọn)

                entity.Property(c => c.Description)
                    .HasMaxLength(500); // Giới hạn độ dài mô tả

                entity.Property(c => c.CreatedDate)
                    .HasDefaultValueSql("GETDATE()"); // Gán giá trị mặc định là ngày hiện tại

                entity.HasMany(c => c.TaskJobs) // Một Category có nhiều TaskJob
                    .WithOne(t => t.Category) // Một TaskJob thuộc về một Category
                    .HasForeignKey(t => t.CategoryId) // Thiết lập khóa ngoại
                    .OnDelete(DeleteBehavior.Cascade); // Xóa TaskJob khi xóa Category
            });

            // Cấu hình cho TaskJob
            modelBuilder.Entity<TaskJob>(entity =>
            {
                entity.Property(t => t.Id)
                    .ValueGeneratedOnAdd(); // ID tự tăng

                entity.Property(t => t.Title)
                    .IsRequired() // Không cho phép null
                    .HasMaxLength(200); // Giới hạn độ dài tiêu đề

                entity.Property(t => t.Description)
                    .HasMaxLength(1000); // Giới hạn độ dài mô tả

                entity.Property(t => t.Status)
                    .HasMaxLength(50); // Giới hạn độ dài trạng thái

                entity.Property(t => t.Priority)
                    .HasMaxLength(50); // Giới hạn độ dài mức độ ưu tiên

                entity.Property(t => t.CreatedDate)
                    .HasDefaultValueSql("GETDATE()"); // Gán giá trị mặc định là ngày hiện tại

                // Cấu hình quan hệ giữa TaskJob và Category
                entity.HasOne(t => t.Category)
                    .WithMany(c => c.TaskJobs) // Một Category có nhiều TaskJob
                    .HasForeignKey(t => t.CategoryId) // Thiết lập khóa ngoại
                    .OnDelete(DeleteBehavior.Cascade); // Xóa TaskJob khi xóa Category
            });
        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}