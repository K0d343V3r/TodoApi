using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoList
    {
        public string Name { get; set; }
        public List<TodoItem> Items { get; private set; }

        TodoList()
        {
            Items = new List<TodoItem>();
        }
    }
}
