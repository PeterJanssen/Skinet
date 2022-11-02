using System;
using System.Text.Json.Serialization;
using Domain.Models.AccountModels.AppUserModels;

namespace Domain.Models.AccountModels.JWT
{
    public class RefreshToken : BaseModel
    {
        [JsonPropertyName("appUser")]
        public AppUser AppUser { get; set; }

        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddDays(7);

        [JsonPropertyName("isExpired")]
        public bool IsExpired => DateTime.UtcNow >= ExpireAt;

        [JsonPropertyName("revoked")]
        public DateTime? Revoked { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
