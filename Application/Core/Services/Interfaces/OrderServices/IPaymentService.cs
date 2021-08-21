using Domain.Models.BasketModels;
using Domain.Models.OrderModels;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.OrderServices
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}