using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Models
{
    public abstract class TodoBrowsingElement : TodoElementBase
    {
        public int RemainingCount { get; internal set; }
    }
}
