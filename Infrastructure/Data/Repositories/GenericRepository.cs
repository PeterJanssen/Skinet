using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Specifications;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _storeContext;
        public GenericRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _storeContext.Set<T>().FindAsync(id);
        }
        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _storeContext.Set<T>().ToListAsync();
        }
        public async Task<T> GetEntityWithSpec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }
        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(
                _storeContext.Set<T>().AsQueryable(),
                 specification
                 );
        }

        public void Add(T entity)
        {
            _storeContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _storeContext.Set<T>().Attach(entity);
            _storeContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _storeContext.Set<T>().Remove(entity);
        }
    }
}