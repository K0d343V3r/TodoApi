using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public abstract class TodoBrowsingElement : TodoElementBase
    {
        public int RemainingCount { get; internal set; }
    }
}
