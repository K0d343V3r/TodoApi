using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoElement : TodoBrowsingElement, ISortable
    {
        public int Position { get; set; }
    }
}
