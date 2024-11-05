using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todolist.DAL.Entities;

namespace Todolist.DAL.Repositories
{
    public class TaskRepo
    {

        private TodoApplicationPrn212Context _context;

        public List<TaskJob> GetAll()
        {
            _context = new TodoApplicationPrn212Context();
            return _context.TaskJobs.Include("Category").ToList();
        }

        public void Add(TaskJob entity)
        {
            _context = new TodoApplicationPrn212Context();
            _context.TaskJobs.Add(entity);
            _context.SaveChanges();
        }

        public void Update(TaskJob entity)
        {
            _context = new TodoApplicationPrn212Context();
            _context.TaskJobs.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(TaskJob entity)
        {
            _context = new TodoApplicationPrn212Context();
            _context.TaskJobs.Remove(entity);
            _context.SaveChanges();
        }

    }
}
