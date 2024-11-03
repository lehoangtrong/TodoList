using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todolist.DAL;
using Todolist.DAL.Entities;


namespace TodoList.DAL.Repositories
{
   
    public class CategoryRepository
    {
        private TodoApplicationPrn212Context _context;

        public List<Category> GetAll() 
        {
            _context = new();
            return _context.Categories.ToList();
        }

    }
}
