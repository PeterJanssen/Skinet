using Application.Core.Specifications.OrderSpec;
using Domain.Models.BasketModels;
using Domain.Models.OrderModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.OrderServices
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, DeliveryMethod deliveryMethod, CustomerBasket customerBasket, OrderAddress shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(OrderSpecParams specParams, string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<int> CountProductsAsync(OrderSpecParams specParams, string email);
    }
}