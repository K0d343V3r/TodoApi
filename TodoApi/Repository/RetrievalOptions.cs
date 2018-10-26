﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public class RetrievalOptions<T>
    {
        public OrderByOptions<T> OrderBy = new OrderByOptions<T>();
        public Expression<Func<T, bool>> Where { get; set; }
    }
}
