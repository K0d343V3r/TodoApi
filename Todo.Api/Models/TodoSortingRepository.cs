using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoSortingRepository<T> : DbSortingRepository<T> where T : EntityBase, ISortable
    {
        public TodoSortingRepository(TodoContext context)
            : base(context)
        {
        }
    }
}
