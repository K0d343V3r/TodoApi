using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoQueryPredicate : EntityBase, ISortable, IUpdatable<TodoQueryPredicate>
    {
        public QueryOperand Operand { get; set; }
        public QueryOperator Operator { get; set; }
        public bool? BoolValue { get; set; }
        public DateTime? AbsoluteDateValue { get; set; }
        public int? RelativeDateValue { get; set; }
        public QueryKeyword? Keyword { get; set; }
        public QueryPredicateGroup? Group { get; set; }
        public int Position { get; set; }
        public int TodoQueryId { get; set; }

        public void UpdateFrom(TodoQueryPredicate fromPredicate)
        {
            Operand = fromPredicate.Operand;
            Operator = fromPredicate.Operator;
            BoolValue = fromPredicate.BoolValue;
            AbsoluteDateValue = fromPredicate.AbsoluteDateValue;
            RelativeDateValue = fromPredicate.RelativeDateValue;
            Keyword = fromPredicate.Keyword;
            Group = fromPredicate.Group;
            Position = fromPredicate.Position;
            TodoQueryId = fromPredicate.TodoQueryId;
        }
    }
}
