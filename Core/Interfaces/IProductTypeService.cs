using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductTypesService
    {
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
    }
}