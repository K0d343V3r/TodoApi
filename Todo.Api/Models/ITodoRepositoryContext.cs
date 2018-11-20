using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public interface ITodoRepositoryContext : IRepositoryContext
    {
        IRepository<TodoList> TodoLists { get; }
        IRepository<TodoListItem> TodoItems { get; }
        IRepository<TodoQuery> TodoQueries { get; }
        IRepository<TodoItemReference> TodoReferences { get; }
    }
}
