using System;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoListItem : EntityBase
    {
        public string Task { get; set; }
        public bool Done { get; set; }
        public DateTime DueDate { get; set; } = default(DateTime);
        public int TodoListId { get; set; }
    }
}