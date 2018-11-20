using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Api.Repository;

namespace Todo.Api.Models
{
    public class TodoQueryPredicate : EntityBase
    {
        public QueryOperand Operand { get; set; }
        public QueryOperator Operator { get; set; }
        public bool? BoolValue { get; set; }
        public DateTime? AbsoluteDateValue { get; set; }
        public int? RelativeDateValue { get; set; }
        public QueryKeyword? Keyword { get; set; }
        public QueryPredicateGroup? Group { get; set; }
        public int TodoQueryId { get; set; }
    }
}
