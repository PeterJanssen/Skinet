using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams);
        Task<int> CountProductsAsync(ProductSpecParams specParams);
        Task<Product> GetProductByIdAsync(int id);
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
        Task<int> CreateProductAsync(Product productToCreate);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(Product product);
    }
}