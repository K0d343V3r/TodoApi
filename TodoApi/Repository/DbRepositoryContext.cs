using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Repository
{
    public class DbRepositoryContext : IRepositoryContext
    {   
        protected readonly DbContext _context;

        public DbRepositoryContext(DbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
