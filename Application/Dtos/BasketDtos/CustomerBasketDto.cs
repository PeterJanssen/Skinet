using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.BasketDtos
{
    public class CustomerBasketDto
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("items")]
        public List<BasketItemDto> Items { get; set; }
        [JsonPropertyName("deliveryMethodId")]
        public int? DeliveryMethodId { get; set; }
        [JsonPropertyName("clientSecret")]
        public string ClientSecret { get; set; }
        [JsonPropertyName("paymentIntentId")]
        public string PaymentIntentId { get; set; }
        [JsonPropertyName("shippingPrice")]
        public decimal ShippingPrice { get; set; }
    }
}