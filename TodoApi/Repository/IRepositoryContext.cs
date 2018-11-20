using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Api.Repository
{
    public interface IRepositoryContext
    {
        Task SaveChangesAsync();
    }
}
