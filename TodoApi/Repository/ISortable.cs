using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Repository
{
    public interface ISortable
    {
        int Position { get; set; }
    }
}
