using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;

namespace API.Controllers.AccountControllers
{
    public class BaseAccountController : BaseApiController
    {
        protected LoginResult CreateLoginResult(
            string email,
            string displayName,
            List<string> roles,
            string originalUsername,
            string accessToken,
            string refreshToken
            )
        {
            return new LoginResult
            {
                Email = email,
                Roles = roles,
                DisplayName = displayName,
                OriginalUserName = originalUsername,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        protected List<Claim> GetClaimsForUser(AppUser user, IList<string> userRoles)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }
    }
}