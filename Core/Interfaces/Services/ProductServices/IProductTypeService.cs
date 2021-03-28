using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.ProductEntities;

namespace Core.Interfaces.Services.ProductServices
{
    public interface IProductTypesService
    {
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
    }
}