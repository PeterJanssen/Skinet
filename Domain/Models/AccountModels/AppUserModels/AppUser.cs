using System;
using System.Collections.Generic;
using Domain.Models.AccountModels.JWT;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.AccountModels.AppUserModels
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}