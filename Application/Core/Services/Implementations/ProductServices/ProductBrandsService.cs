using Application.Core.Services.Interfaces.ProductServices;
using Domain.Models.ProductModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.ProductServices
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