using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.Identity
{
    public interface IUserService
    {
        Task<AppUser> GetUser(string userName);
        Task<SignInResult> SignUserIn(AppUser user, string password);
        Task<IList<string>> GetUserRoles(AppUser user);
        Task<IdentityResult> CreateAsync(AppUser user, string password);
        Task<IdentityResult> AddToRoleAsync(AppUser user);
        Task<AppUser> FindUserByClaimsPrincipleWithAddressAsync(ClaimsPrincipal user);
        Task<IdentityResult> UpdateAsync(AppUser user);
    }
}
