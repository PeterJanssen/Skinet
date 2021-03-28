using System.Threading.Tasks;
using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;

namespace Core.Interfaces.Services.OrderServices
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}