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
        public int Position { get; set; }
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
            Position = list.Position;

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
            for (int i = 0; i < list.Items.Count; i++)
            {
                // ignore passed-in position, sort according to list position
                list.Items[i].Position = i;
                var current = list.Items[i].Id == 0 ? null : Items.FirstOrDefault(t => t.Id == list.Items[i].Id);
                if (current == null)
                {
                    Items.Add(list.Items[i]);
                }
                else
                {
                    current.UpdateFrom(list.Items[i]);
                }
            }
        }
    }
}