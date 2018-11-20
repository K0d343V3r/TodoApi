using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Todo.Api.Repository;

namespace Todo.Api.Models
{
    public class TodoRepository<T> : DbRepository<T> where T : EntityBase
    {
        public TodoRepository(TodoContext context)
            : base(context)
        {
        }
    }
}
