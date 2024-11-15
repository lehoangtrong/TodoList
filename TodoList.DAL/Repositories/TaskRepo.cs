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

        public TaskRepo()
        {
            _context = new TodoApplicationPrn212Context();
        }

        public List<TaskJob> GetAll()
        {
            return _context.TaskJobs.Include("Category").ToList();
        }

        public void Add(TaskJob entity)
        {
            _context.TaskJobs.Add(entity);
            _context.SaveChanges();
        }

        public void Update(TaskJob entity)
        {
            // Kiểm tra xem thực thể đã được theo dõi bởi DbContext chưa
            var trackedEntity = _context.TaskJobs.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (trackedEntity != null)
            {
                // Nếu đã được theo dõi, sử dụng Attach và cập nhật giá trị
                _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                // Nếu chưa được theo dõi, dùng Attach để thêm vào DbContext
                _context.TaskJobs.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            // Lưu thay đổi vào database
            _context.SaveChanges();
        }

        public void Delete(TaskJob entity)
        {
            _context.TaskJobs.Remove(entity);
            _context.SaveChanges();
        }
        public List<TaskJob?> Search(string keyword)
        {
            var list = _context.TaskJobs.ToList();
            keyword = keyword.ToLower();

            // Duyệt qua danh sách các task và kiểm tra tất cả các trường
            var result = list.Where(t => t.GetType().GetProperties()
            .Any(prop => prop.GetValue(t) != null && prop.GetValue(t).ToString().ToLower().Contains(keyword))).ToList();

            if (result.Count <= 0 || result == null)
            {
                return null;
            }
            return result;
        }
    }
}
