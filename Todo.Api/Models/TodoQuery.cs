using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Models
{
    public class TodoQuery : TodoElementBase
    {
        public List<TodoQueryPredicate> Predicates { get; set; } = new List<TodoQueryPredicate>();
        public QueryOperand? OrderBy { get; set; }
        public QueryDirection? OrderByDirection { get; set; }
    }
}
