using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public class RetrievalOptions<T>
    {
        public Expression<Func<T, bool>> WherePredicate { get; set; }
        public Expression<Func<T, object>> OrderByPredicate { get; set; }
    }
}
