using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Todo.Api.Repository
{
    public class OrderByOptions<T>
    {
        public bool Ascending { get; set; } = true;
        public Expression<Func<T, object>> Predicate { get; set; }
    }
}
