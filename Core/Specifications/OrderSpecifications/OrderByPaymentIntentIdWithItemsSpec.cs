using Core.Entities.OrderEntities;

namespace Core.Specifications.OrderSpecifications
{
    public class OrderByPaymentIntentIdSpec : BaseSpecification<Order>
    {
        public OrderByPaymentIntentIdSpec(string paymentIntentId) : base(o => o.PaymentIntentId == paymentIntentId)
        {
        }
    }
}