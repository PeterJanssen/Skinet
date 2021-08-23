using System.Text.Json.Serialization;

namespace Application.Dtos.OrderDtos
{
    public class OrderItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }
        [JsonPropertyName("pictureUrl")]
        public string PictureUrl { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}