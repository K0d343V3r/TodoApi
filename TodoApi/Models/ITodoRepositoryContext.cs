using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public interface ITodoRepositoryContext : IRepositoryContext
    {
        IRepository<TodoList> TodoLists { get; }
        IRepository<TodoListItem> TodoItems { get; }
    }
}
