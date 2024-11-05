using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todolist.DAL.Entities;
using TodoList.DAL.Repositories;

namespace Todolist.BLL.Services
{
    
    public class CategoryService
    {
        private CategoryRepository _repository = new();

        public List<Category> GetAllCategories()
        {
            return _repository.GetAll();
        }
    }
}
