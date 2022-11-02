using Application.Core.Services.Interfaces.Identity.JWT;
using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;
using Domain.Models.AccountModels.Google;
using Domain.Models.AccountModels.JWT;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Services.Implementations.Identity.JWT
{
    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly GoogleAuthSettings _googleSettings;
        private readonly byte[] _secret;

        public JwtAuthManager(JwtTokenConfig jwtTokenConfig, GoogleAuthSettings googleAuthSettings)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _googleSettings = googleAuthSettings;
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }
        public string GenerateToken(AppUser appUser, List<Claim> claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new SecurityTokenDescriptor
            {
                Issuer = _jwtTokenConfig.Issuer,
                Audience = shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(jwtToken);

            return tokenHandler.WriteToken(token);
        }

        public RefreshToken Refresh(AppUser appuser, string accessToken, DateTime now)
        {
            var (principal, jwtToken) = DecodeJwtToken(accessToken);

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return new RefreshToken
            {
                AppUser = appuser,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
            };
        }
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleLoginRequest googleLoginRequest)
        {
            if (string.IsNullOrEmpty(_googleSettings.ClientId))
            {
                return null;
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleSettings.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginRequest.IdToken, settings);
            return payload;
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }
        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
