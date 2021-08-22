using System.Text.Json.Serialization;

namespace Application.Dtos.OrderDtos
{
    public class DeliveryMethodDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }
        [JsonPropertyName("deliveryTime")]
        public string DeliveryTime { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}