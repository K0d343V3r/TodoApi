using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoItemReference : EntityBase, ISortable, IUpdatable<TodoItemReference>
    {
        public int Position { get; set; }
        public TodoListItem Item { get; set; }
        public int TodoQueryId { get; set; }

        public void UpdateFrom(TodoItemReference fromReference)
        {
            // we only update positions on references
            Position = fromReference.Position;
        }
    }
}
