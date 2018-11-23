using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoList : TodoElementBase, ISortable, IUpdatable<TodoList>
    {
        public int Position { get; set; }
        public List<TodoListItem> Items { get; set; } = new List<TodoListItem>();

        public TodoElement ToElement()
        {
            var element = new TodoElement
            {
                Id = Id,
                Name = Name,
                Position = Position,
                RemainingCount = Items.Count(i => !i.Done)
            };
            return element;
        }

        public void UpdateFrom(TodoElement fromElement)
        {
            base.UpdateFrom(fromElement);

            Position = fromElement.Position;
        }

        public void UpdateFrom(TodoList fromList)
        {
            base.UpdateFrom(fromList);

            Position = fromList.Position;

            CollectionUpdater<TodoListItem>.Update(Items, fromList.Items);
        }
    }
}