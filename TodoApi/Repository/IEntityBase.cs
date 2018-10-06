using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public interface IEntityBase
    {
        long Id { get; set; }
        int Position { get; set; }
    }
}
