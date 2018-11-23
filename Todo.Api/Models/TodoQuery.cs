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
            var element = new TodoQueryElement
            {
                Id = Id,
                Name = Name,
                Query = this,
                RemainingCount = remainingCount
            };
            return element;
        }

        public void UpdateFrom(TodoQueryElement fromElement)
        {
            base.UpdateFrom(fromElement);

            // nothing to update other than in base class
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
