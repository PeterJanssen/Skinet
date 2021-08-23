using Application.Dtos.AccountDtos;
using System.Text.Json.Serialization;

namespace Application.Dtos.OrderDtos
{
    public class OrderDto
    {
        [JsonPropertyName("basketId")]
        public string BasketId { get; set; }
        [JsonPropertyName("deliveryMethodId")]
        public int DeliveryMethodId { get; set; }
        [JsonPropertyName("shipToAddress")]
        public AddressDto ShipToAddress { get; set; }
    }
}