using Application.Core.Services.Interfaces.ProductServices;
using Application.Core.Specifications.ProductSpec;
using Domain.Models.ProductModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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