using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;
using Domain.Models.AccountModels.JWT;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.Identity.JWT
{
    public interface IJwtAuthManager
    {
        string GenerateToken(AppUser appUser, List<Claim> claims, DateTime now);
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleLoginRequest googleLoginRequest);
        RefreshToken Refresh(AppUser appuser, string accessToken, DateTime now);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }
}
