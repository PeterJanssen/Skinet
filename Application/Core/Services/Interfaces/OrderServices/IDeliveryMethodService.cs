using Domain.Models.OrderModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.OrderServices
{
    public interface IDeliveryMethodService
    {
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<DeliveryMethod> GetDeliveryMethodByIdAsync(int deliveryMethodId);
    }
}