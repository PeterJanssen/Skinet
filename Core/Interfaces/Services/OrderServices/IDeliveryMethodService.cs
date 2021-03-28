using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderEntities;

namespace Core.Interfaces.Services.OrderServices
{
    public interface IDeliveryMethodService
    {
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<DeliveryMethod> GetDeliveryMethodByIdAsync(int deliveryMethodId);
    }
}