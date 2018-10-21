using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoItemReference : EntityBase
    {
        public TodoListItem Item { get; set; }
        public int TodoQueryId { get; set; }
    }
}
