using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoList : TodoElementBase, ISortable
    {
        public int Position { get; set; }
        public List<TodoListItem> Items { get; set; } = new List<TodoListItem>();
    }
}