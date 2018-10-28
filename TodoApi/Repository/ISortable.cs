using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public interface ISortable
    {
        int Position { get; set; }
    }
}
