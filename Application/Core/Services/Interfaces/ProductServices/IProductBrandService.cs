using Domain.Models.ProductModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.ProductServices
{
    public interface IProductBrandsService
    {
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
    }
}