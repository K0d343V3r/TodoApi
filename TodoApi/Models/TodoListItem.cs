using System;
using Todo.Api.Repository;

namespace Todo.Api.Models
{
    public class TodoListItem : EntityBase, ISortable
    {
        public int Position { get; set; }
        public string Task { get; set; }
        public bool Done { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Important { get; set; }
        public int TodoListId { get; set; }
    }
}