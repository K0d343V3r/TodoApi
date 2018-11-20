using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Models
{
    public class TodoQueryResults
    {
        public int TodoQueryId { get; set; }
        public List<TodoItemReference> References { get; set; }
    }
}
