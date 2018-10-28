using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public class DbSortingRepository<T> : IRepository<T> where T : EntityBase, ISortable
    {
        private DbRepository<T> _repository;

        public DbSortingRepository(DbContext context)
        {
            _repository = new DbRepository<T>(context);
        }

        public Task AddAsync(T entity)
        {
            return _repository.AddAsync(entity);
        }

        public int Count()
        {
            return _repository.Count();
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public Task<List<T>> GetAsync(params Expression<Func<T, object>>[] includePredicates)
        {
            RetrievalOptions<T> options = new RetrievalOptions<T>();
            options.OrderBy.Predicate = e => e.Position;
          
            return _repository.GetAsync(options, includePredicates);
        }

        public Task<List<T>> GetAsync(Expression<Func<T, bool>> wherePredicate, params Expression<Func<T, object>>[] includePredicates)
        {
            RetrievalOptions<T> options = new RetrievalOptions<T>();
            options.OrderBy.Predicate = e => e.Position;
            options.Where = wherePredicate;

            return _repository.GetAsync(options, includePredicates);
        }

        public Task<List<T>> GetAsync(RetrievalOptions<T> options, params Expression<Func<T, object>>[] includePredicates)
        {
            return _repository.GetAsync(options, includePredicates);
        }

        public Task<T> GetAsync(int id, params Expression<Func<T, object>>[] includePredicates)
        {
            return _repository.GetAsync(id, includePredicates);
        }

        public void Update(T entity)
        {
            _repository.Update(entity);
        }
    }
}
