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
        public List<TaskJob?> Search(string keyword)
        {
            _context = new TodoApplicationPrn212Context();
            var list = _context.TaskJobs.ToList();
            keyword = keyword.ToLower();

            // Duyệt qua danh sách các task và kiểm tra tất cả các trường
            var result = list.Where(t => t.GetType().GetProperties()
            .Where(prop => prop.Name != nameof(TaskJob.Id))
            .Any(prop => prop.GetValue(t) != null && prop.GetValue(t).ToString().ToLower().Contains(keyword))).ToList();

            if (result.Count <= 0 || result == null)
            {
                return null;
            }
            return result;
        }
    }
}
