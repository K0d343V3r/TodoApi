﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoListItem> TodoItems { get; set; }
        public DbSet<TodoQuery> TodoQueries { get; set; }
        public DbSet<TodoQueryResult> TodoResults { get; set; }
    }
}
