using Application.Core.Services.Interfaces.OrderServices;
using Domain.Models.BasketModels;
using Domain.Models.OrderModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Application.Core.Specifications.OrderSpec;
using Stripe;
using Order = Domain.Models.OrderModels.Order;
using Product = Domain.Models.ProductModels.Product;

namespace Application.Core.Services.OrderServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        public PaymentService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IConfiguration config)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null)
            {
                return null;
            }

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                    .GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var amount = (long)basket.Items
                        .Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100;

                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var amount = (long)basket.Items
                    .Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100;
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = amount
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpec(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null)
            {
                return null;
            }

            order.Status = OrderStatus.PaymentFailed;

            await _unitOfWork.Complete();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpec(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null)
            {
                return null;
            }

            order.Status = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }
    }
}