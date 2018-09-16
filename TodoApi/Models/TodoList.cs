using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoList
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<TodoListItem> Items { get; set; }

        public TodoList()
        {
            Items = new List<TodoListItem>();
        }

        public void UpdateFrom(TodoList list)
        {
            // update properties
            Id = list.Id;
            Name = list.Name;

            // remove obsolete child items
            List<TodoListItem> items = new List<TodoListItem>(Items);
            foreach (var item in items)
            {
                if (!list.Items.Any(i => i.Id == item.Id))
                {
                    Items.Remove(item);
                }
            }

            // update existing or add new child items
            foreach (var item in list.Items)
            {
                var current = item.Id == 0 ? null : Items.FirstOrDefault(i => i.Id == item.Id);
                if (current == null)
                {
                    Items.Add(item);
                }
                else
                {
                    current.UpdateFrom(item);
                }
            }
        }
    }
}