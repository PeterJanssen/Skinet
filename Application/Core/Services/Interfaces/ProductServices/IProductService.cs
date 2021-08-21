using Application.Core.Specifications.ProductSpec;
using Domain.Models.ProductModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.ProductServices
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams);
        Task<int> CountProductsAsync(ProductSpecParams specParams);
        Task<Product> GetProductByIdAsync(int id);
        Task<int> CreateProductAsync(Product productToCreate);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(Product product);
    }
}