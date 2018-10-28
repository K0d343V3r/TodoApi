using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoQueryElement : TodoBrowsingElement
    {
        public TodoQuery Query { get; set; }
    }
}
