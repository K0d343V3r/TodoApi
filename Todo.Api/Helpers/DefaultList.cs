using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Api.Models;
using Api.Common.Repository;

namespace Todo.Api.Helpers
{
    internal static class DefaultList
    {
        public static readonly int id = 1;

        public static async void CreateAsync(ITodoRepositoryContext context)
        {
            if (context.TodoLists.Count() == 0)
            {
                await context.TodoLists.AddAsync(new TodoList() { Name = "Default" });
                await context.SaveChangesAsync();
            }
        }
    }
}
