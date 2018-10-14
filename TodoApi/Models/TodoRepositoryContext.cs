using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoRepositoryContext : DbRepositoryContext, ITodoRepositoryContext
    {
        public IRepository<TodoList> TodoLists { get; private set; }
        public IRepository<TodoListItem> TodoItems { get; private set; }
        public IRepository<TodoQuery> TodoQueries { get; private set; }

        public TodoRepositoryContext(TodoContext context)
            : base(context)
        {
            TodoLists = new TodoRepository<TodoList>((TodoContext)_context);
            TodoItems = new TodoRepository<TodoListItem>((TodoContext)_context);
            TodoQueries = new TodoRepository<TodoQuery>((TodoContext)_context);
        }
    }
}
