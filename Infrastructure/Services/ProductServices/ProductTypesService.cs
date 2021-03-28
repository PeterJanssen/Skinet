using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.ProductEntities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services.ProductServices;

namespace Infrastructure.Services.ProductServices
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