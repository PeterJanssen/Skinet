using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.BasketDtos
{
    public class BasketItemDto
    {
        [Required]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Required]
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [Required]
        [JsonPropertyName("pictureUrl")]
        public string PictureUrl { get; set; }
        [Required]
        [JsonPropertyName("brand")]
        public string Brand { get; set; }
        [Required]
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}