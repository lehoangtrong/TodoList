using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todolist.DAL.Entities
{
    public class TodoData
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<TaskJob> TaskJobs { get; set; } = new List<TaskJob>();
    }
}
