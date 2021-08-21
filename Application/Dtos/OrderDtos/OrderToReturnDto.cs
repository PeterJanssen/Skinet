using Domain.Models.OrderModels;
using System;
using System.Collections.Generic;

namespace Application.Dtos.OrderDtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderAddress ShipToAddress { get; set; }
        public string DeliverMethod { get; set; }
        public decimal ShippingPrice { get; set; }
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}