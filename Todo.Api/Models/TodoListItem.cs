using System;
using Api.Common;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoListItem : EntityBase, ISortable, IUpdatable<TodoListItem>
    {
        public int Position { get; set; }
        public string Task { get; set; }
        public bool Done { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Important { get; set; }
        public int TodoListId { get; set; }

        public void UpdateFrom(TodoListItem fromItem)
        {
            Position = fromItem.Position;
            Task = fromItem.Task;
            Done = fromItem.Done;
            DueDate = fromItem.DueDate;
            Important = fromItem.Important;
            TodoListId = fromItem.TodoListId;
        }
    }
}