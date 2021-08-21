using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.AccountDtos
{
    public class LoginRequest
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}