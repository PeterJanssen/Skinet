using System;
using System.Collections.Generic;

namespace Domain.Models.OrderModels
{
    public class Order : BaseModel
    {
        public Order()
        {
        }

        public Order(
            IReadOnlyList<OrderItem> orderItems,
            string buyerEmail,
            OrderAddress shippedToAddress,
            DeliveryMethod deliverMethod,
            decimal subTotal,
            string paymentIntentId)
        {
            OrderItems = orderItems;
            BuyerEmail = buyerEmail;
            ShipToAddress = shippedToAddress;
            DeliverMethod = deliverMethod;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderAddress ShipToAddress { get; set; }
        public DeliveryMethod DeliverMethod { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; }
        public decimal GetTotal()
        {
            return SubTotal + DeliverMethod.Price;
        }
    }
}