using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Data.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> GetEntityWithSpec(ISpecification<T> specification);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification);
        Task<int> CountAsync(ISpecification<T> spec);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}