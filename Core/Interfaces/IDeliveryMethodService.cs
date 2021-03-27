using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
    public interface IDeliveryMethodService
    {
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    }
}