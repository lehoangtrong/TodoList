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

        public List<TaskJob>? GetTodayTasks()
        {
            List<TaskJob> tasks = _repo.GetAll();
            List<TaskJob> todayTasks = tasks.Where(x => x.DueDate == DateTime.Today).ToList();

            return todayTasks;
        }


    }
}
