using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoList : ITodoListBase
    {
        public long Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public List<TodoListItem> Items { get; set; }

        public TodoList()
        {
            Items = new List<TodoListItem>();
        }
    }
}