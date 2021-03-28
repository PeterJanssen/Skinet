using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.ProductEntities;

namespace Core.Interfaces.Services.ProductServices
{
    public interface IProductBrandsService
    {
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
    }
}