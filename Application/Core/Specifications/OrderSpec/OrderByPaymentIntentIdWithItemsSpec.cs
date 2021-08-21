using Domain.Models.OrderModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.OrderSpec
{
    public class OrderByPaymentIntentIdSpec : Specification<Order>
    {
        public OrderByPaymentIntentIdSpec(string paymentIntentId) : base(o => o.PaymentIntentId == paymentIntentId)
        {
        }
    }
}