using Application.Core.Services.Interfaces.OrderServices;
using Application.Core.Specifications.OrderSpec;
using Application.Core.Specifications.ProductSpec;
using Domain.Models.BasketModels;
using Domain.Models.OrderModels;
using Domain.Models.ProductModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        public OrderService(
        IUnitOfWork unitOfWork,
        IBasketRepository basketRepo,
        IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
            _paymentService = paymentService;
        }

        public async Task<int> CountProductsAsync(OrderSpecParams specParams, string email)
        {
            var countSpec = new OrderWithFiltersForCountSpecification(specParams, email);

            var totalItems = await _unitOfWork.Repository<Order>().CountAsync(countSpec);

            return totalItems;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, DeliveryMethod deliveryMethod, CustomerBasket customerBasket, OrderAddress shippingAddress)
        {
            var items = new List<OrderItem>();

            foreach (var item in customerBasket.Items)
            {
                var specification = new ProductsWithTypesAndBrandsSpecification(item.Id);

                var productItem = await _unitOfWork.Repository<Product>().GetEntityWithSpec(specification);
                var itemOrdered = new ProductItemOrdered(
                                    productItem.Id,
                                    productItem.Name,
                                    productItem.Photos.FirstOrDefault(x => x.IsMain)?.PictureUrl
                                    );
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);

                items.Add(orderItem);
            }

            var subtotal = items.Sum(item => item.Price * item.Quantity);

            var spec = new OrderByPaymentIntentIdSpec(customerBasket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(customerBasket.Id);
            }

            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, customerBasket.PaymentIntentId);

            _unitOfWork.Repository<Order>().Add(order);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return null;
            }

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(OrderSpecParams specParams, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(specParams, buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}