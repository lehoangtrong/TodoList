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

        public List<Category> GetAll()
        {
            _context = new TodoApplicationPrn212Context();
            return _context.Categories.ToList();
        }

        public void Add(Category entity)
        {
            _context = new TodoApplicationPrn212Context();
            _context.Categories.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Category entity)
        {
            _context = new TodoApplicationPrn212Context();
            _context.Categories.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Category entity)
        {
            _context = new TodoApplicationPrn212Context();
            _context.Categories.Remove(entity);
            _context.SaveChanges();
        }

    }
}
