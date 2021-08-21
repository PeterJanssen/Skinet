using Domain.Models;
using System;
using System.Threading.Tasks;

namespace Persistence.Data.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseModel;
        Task<int> Complete();
    }
}