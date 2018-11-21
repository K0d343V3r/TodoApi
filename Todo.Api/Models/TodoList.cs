using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoList : TodoElementBase, ISortable
    {
        public int Position { get; set; }
        public List<TodoListItem> Items { get; set; } = new List<TodoListItem>();

        public TodoElement ToElement()
        {
            var element = new TodoElement();
            element.UpdateFrom(this);
            element.RemainingCount = Items.Count(i => !i.Done);
            return element;
        }

        public void UpdateFrom(TodoList fromList)
        {
            base.UpdateFrom(fromList);

            // remove obsolete child items
            List<TodoListItem> items = new List<TodoListItem>(Items);
            foreach (var item in items)
            {
                if (!fromList.Items.Any(i => i.Id == item.Id))
                {
                    Items.Remove(item);
                }
            }

            // update existing or add new child items
            for (int i = 0; i < fromList.Items.Count; i++)
            {
                var current = fromList.Items[i].Id == 0 ? null :
                    Items.FirstOrDefault(t => t.Id == fromList.Items[i].Id);
                if (current == null)
                {
                    Items.Add(fromList.Items[i]);
                }
                else
                {
                    current.UpdateFrom(fromList.Items[i]);
                }
            }
        }
    }
}