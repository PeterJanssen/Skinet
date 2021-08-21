using Application.Core.Services.Interfaces.OrderServices;
using Domain.Models.OrderModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.OrderServices
{
    public class DeliveryMethodService : IDeliveryMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeliveryMethodService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }
        public async Task<DeliveryMethod> GetDeliveryMethodByIdAsync(int deliveryMethodId)
        {
            return await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
        }

    }
}