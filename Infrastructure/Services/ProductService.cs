using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().ListAllAsync();

            return brands;
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpec(specification);

            return product;
        }
        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(specParams);

            var products = await _unitOfWork.Repository<Product>().ListAsync(specification);

            return products;
        }
        public async Task<int> CountProductsAsync(ProductSpecParams specParams)
        {
            var countSpec = new ProductWithFiltersForCountSpecification(specParams);

            var totalItems = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

            return totalItems;
        }
        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            var types = await _unitOfWork.Repository<ProductType>().ListAllAsync();

            return types;
        }
        public async Task<int> CreateProductAsync(Product productToCreate)
        {
            _unitOfWork.Repository<Product>().Add(productToCreate);

            var result = await _unitOfWork.Complete();

            return result;
        }
        public async Task<int> UpdateProductAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Update(product);

            var result = await _unitOfWork.Complete();

            return result;
        }
        public async Task<int> DeleteProductAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Delete(product);

            var result = await _unitOfWork.Complete();

            return result;
        }
    }
}