using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Models
{
    public class TodoQueryElement : TodoBrowsingElement
    {
        public TodoQuery Query { get; set; }
    }
}
