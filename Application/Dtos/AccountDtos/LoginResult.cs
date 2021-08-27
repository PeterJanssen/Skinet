using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Dtos.AccountDtos
{
    public class LoginResult
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("role")]
        public List<string> Roles { get; set; }

        [JsonPropertyName("originalUserName")]
        public string OriginalUserName { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
