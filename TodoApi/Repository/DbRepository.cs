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
            if (options.WherePredicate != null)
            {
                set.Where(options.WherePredicate);
            }
            if (options.OrderByPredicate != null)
            {
                set.OrderBy(options.OrderByPredicate);
            }
            else
            {
                set.OrderBy(t => t.Position);
            }
            foreach (var predicate in includePredicates)
            {
                set = set.Include(predicate);
            }

            return await set.ToListAsync();
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
