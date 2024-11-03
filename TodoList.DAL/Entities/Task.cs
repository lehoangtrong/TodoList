using System;
using System.Collections.Generic;

namespace Todolist.DAL.Entities;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public string? Priority { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
