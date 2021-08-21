using Application.Core.Services.Interfaces.ProductServices;
using Domain.Models.ProductModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.ProductServices
{
    public class ProductTypesService : IProductTypesService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductTypesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            var types = await _unitOfWork.Repository<ProductType>().ListAllAsync();

            return types;
        }
    }
}