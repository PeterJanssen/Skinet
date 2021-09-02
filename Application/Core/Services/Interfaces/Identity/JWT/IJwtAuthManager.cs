using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.JWT;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.Identity.JWT
{
    public interface IJwtAuthManager
    {
        IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }
        JwtAuthResult GenerateTokens(string username, List<Claim> claims, DateTime now);
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleLoginRequest googleLoginRequest);
        JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);
        void RemoveExpiredRefreshTokens(DateTime now);
        void RemoveRefreshTokenByUserName(string userName);
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }
}
