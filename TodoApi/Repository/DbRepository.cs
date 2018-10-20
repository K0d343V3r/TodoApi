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

        public int Count()
        {
            return _context.Set<T>().Count();
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
            return await GetAsync(new RetrievalOptions<T>(), includePredicates);
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> wherePredicate, params Expression<Func<T, object>>[] includePredicates)
        {
            var options = new RetrievalOptions<T>()
            {
                WherePredicate = wherePredicate
            };
            return await GetAsync(options, includePredicates);
        }

        public async Task<List<T>> GetAsync(RetrievalOptions<T> options, params Expression<Func<T, object>>[] includePredicates)
        {
            IQueryable<T> set = _context.Set<T>();
            foreach (var predicate in includePredicates)
            {
                set = set.Include(predicate);
            }

            if (options.WherePredicate != null)
            {
                return await set.Where(options.WherePredicate).OrderBy(options.OrderByPredicate ?? (t => t.Position)).ToListAsync();
            }
            else
            {
                return await set.OrderBy(options.OrderByPredicate ?? (t => t.Position)).ToListAsync();
            }
        }

        public async Task<T> GetAsync(int id, params Expression<Func<T, object>>[] includePredicates)
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
