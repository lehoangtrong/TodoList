using System;
using System.Collections.Generic;

namespace TodoList.DAL.Entities;

public partial class Category
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
