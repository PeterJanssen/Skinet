using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.AccountDtos
{
    public class AddressDto
    {
        [Required]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [Required]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [Required]
        [JsonPropertyName("street")]
        public string Street { get; set; }
        [Required]
        [JsonPropertyName("city")]
        public string City { get; set; }
        [Required]
        [JsonPropertyName("state")]
        public string State { get; set; }
        [Required]
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }
    }
}