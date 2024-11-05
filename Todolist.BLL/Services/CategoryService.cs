using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todolist.DAL.Repositories;
using Todolist.DAL.Entities;

namespace Todolist.BLL.Services
{
    public class CategoryService
    {


        private CategoryRepo _repo = new CategoryRepo();

        public List<Category> GetAllCategorys()
        {
            return _repo.GetAll();
        }

        public void AddCategory(Category entity)
        {
            _repo.Add(entity);
        }

        public void UpdateCategory(Category entity)
        {
            _repo.Update(entity);
        }

        public void RemoveCategory(Category entity)
        {
            _repo.Delete(entity);
        }
    }
}
