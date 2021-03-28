using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class ProductBrandsService : IProductBrandsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductBrandsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().ListAllAsync();

            return brands;
        }
    }
}