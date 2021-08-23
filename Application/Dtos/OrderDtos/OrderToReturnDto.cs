using Domain.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Dtos.OrderDtos
{
    public class OrderToReturnDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("buyerEmail")]
        public string BuyerEmail { get; set; }
        [JsonPropertyName("orderDate")]
        public DateTimeOffset OrderDate { get; set; }
        [JsonPropertyName("shipToAddress")]
        public OrderAddressDto ShipToAddress { get; set; }
        [JsonPropertyName("deliverMethod")]
        public string DeliverMethod { get; set; }
        [JsonPropertyName("shippingPrice")]
        public decimal ShippingPrice { get; set; }
        [JsonPropertyName("orderItems")]
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
        [JsonPropertyName("subTotal")]
        public decimal SubTotal { get; set; }
        [JsonPropertyName("total")]
        public decimal Total { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}