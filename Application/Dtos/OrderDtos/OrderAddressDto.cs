using System.Text.Json.Serialization;

namespace Application.Dtos.OrderDtos
{
    public class OrderAddressDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("street")]
        public string Street { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }
    }
}
