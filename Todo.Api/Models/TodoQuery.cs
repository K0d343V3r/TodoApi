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

        public TodoQueryElement ToElement(int remainingCount)
        {
            var element = new TodoQueryElement();
            element.UpdateFrom(this);
            element.Query = this;
            element.RemainingCount = remainingCount;
            return element;
        }

        public void UpdateFrom(TodoQuery fromQuery)
        {
            base.UpdateFrom(fromQuery);

            OrderBy = fromQuery.OrderBy;
            OrderByDirection = fromQuery.OrderByDirection;

            Predicates.Clear();
            Predicates.AddRange(fromQuery.Predicates);
        }
    }
}
