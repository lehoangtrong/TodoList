using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todolist.DAL.Entities;

namespace Todolist.DAL.Repositories
{
    public class CategoryRepo
    {

        private TodoApplicationPrn212Context _context;

        public CategoryRepo()
        {
            _context = new TodoApplicationPrn212Context();
        }

        public List<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public void Add(Category entity)
        {
            _context.Categories.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Category entity)
        {
            // Kiểm tra xem thực thể đã được theo dõi bởi DbContext chưa
            var trackedEntity = _context.Categories.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (trackedEntity != null)
            {
                // Nếu thực thể đã được theo dõi, cập nhật giá trị
                _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                // Nếu chưa được theo dõi, dùng Attach để thêm vào DbContext
                _context.Categories.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            // Lưu thay đổi vào database
            _context.SaveChanges();
        }

        public void Delete(Category entity)
        {
            _context.Categories.Remove(entity);
            _context.SaveChanges();
        }

    }
}
