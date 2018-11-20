using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public class TodoRepositoryContext : DbRepositoryContext, ITodoRepositoryContext
    {
        public IRepository<TodoList> TodoLists { get; private set; }
        public IRepository<TodoListItem> TodoItems { get; private set; }
        public IRepository<TodoQuery> TodoQueries { get; private set; }
        public IRepository<TodoItemReference> TodoReferences { get; private set; }

        public TodoRepositoryContext(TodoContext context)
            : base(context)
        {
            TodoLists = new TodoSortingRepository<TodoList>((TodoContext)_context);
            TodoItems = new TodoSortingRepository<TodoListItem>((TodoContext)_context);
            TodoQueries = new TodoRepository<TodoQuery>((TodoContext)_context);
            TodoReferences = new TodoSortingRepository<TodoItemReference>((TodoContext)_context);
        }
    }
}
