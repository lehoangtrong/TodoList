using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todolist.DAL.Repositories;
using Todolist.DAL.Entities;

namespace Todolist.BLL.Services
{
    public class TaskService
    {

        private TaskRepo _repo = new TaskRepo();
        public enum TaskType
        {
            Today,
            Upcoming,
            Important,
            Planned,
            Completed,
            All
        }
        public List<TaskJob> GetAllTaskJobs()
        {
            return _repo.GetAll();
        }

        public void AddTaskJob(TaskJob entity)
        {
            _repo.Add(entity);
        }

        public void UpdateTaskJob(TaskJob entity)
        {
            _repo.Update(entity);
        }

        public void RemoveTaskJob(TaskJob entity) // Or DeleteTaskJob
        {
            _repo.Delete(entity);
        }

        public List<TaskJob> GetTasks(TaskType type)
        {
            List<TaskJob> tasks = _repo.GetAll().OrderByDescending(x => x.DueDate).ToList();

            return type switch
            {
                TaskType.Today => tasks.Where(x => x.DueDate == DateTime.Today).ToList(),
                TaskType.Upcoming => tasks.Where(x => x.DueDate > DateTime.Today).ToList(),
                TaskType.Important => tasks.Where(x => x.Priority == "High").ToList(),
                TaskType.Planned => tasks.Where(x => x.Status == "Pending").ToList(),
                TaskType.Completed => tasks.Where(x => x.Status == "Completed").ToList(),
                TaskType.All => tasks.ToList(),
                _ => new List<TaskJob>()
            };
        }

    }
}
