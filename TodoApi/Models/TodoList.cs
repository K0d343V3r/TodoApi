using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoList : TodoElementBase
    {
        public List<TodoListItem> Items { get; set; }

        public TodoList()
        {
            Items = new List<TodoListItem>();
        }
    }
}