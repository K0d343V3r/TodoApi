using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoQuery : TodoElementBase
    {
        public QueryOperand Operand { get; set; }
        public QueryOperator Operator { get; set; }
        public bool BoolValue { get; set; }
        public DateTime DateValue { get; set; }
    }
}
