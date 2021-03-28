using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductBrandsService
    {
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
    }
}