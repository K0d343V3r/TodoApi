using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoList : TodoListInfo
    {
        public List<TodoListItem> Items { get; set; }

        public override int ItemCount => Items != null ? Items.Count : 0;

        public TodoList()
        {
            Items = new List<TodoListItem>();
        }
    }
}