using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public class DbRepository<T> : IRepository<T> where T : EntityBase
    {
        private readonly DbContext _context;

        public DbRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<List<T>> GetAsync(params Expression<Func<T, object>>[] includePredicates)
        {
            IQueryable<T> set = _context.Set<T>();
            foreach (var predicate in includePredicates)
            {
                set = set.Include(predicate);
            }

            return await set.OrderBy(t => t.Position).ToListAsync();
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> wherePredicate, params Expression<Func<T, object>>[] includePredicates)
        {
            IQueryable<T> set = _context.Set<T>();
            foreach (var predicate in includePredicates)
            {
                set = set.Include(predicate);
            }

            return await set.Where(wherePredicate).OrderBy(t => t.Position).ToListAsync();
        }

        public async Task<T> GetAsync(long id, params Expression<Func<T, object>>[] includePredicates)
        {
            IQueryable<T> set = _context.Set<T>();
            foreach (var predicate in includePredicates)
            {
                set = set.Include(predicate);
            }

            return await set.FirstOrDefaultAsync(t => t.Id == id);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
