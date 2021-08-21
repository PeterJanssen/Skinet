using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindUserByClaimsPrincipleWithAddressAsync(this UserManager<AppUser> input, ClaimsPrincipal user)
        {
            var email = ClaimsPrincipleExtensions.RetrieveEmailFromPrincipal(user);

            return await input.Users
            .Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Email == email);
        }

        public static async Task<AppUser> FindUserByClaimsPrincipleAsync(this UserManager<AppUser> input, ClaimsPrincipal user)
        {
            var email = ClaimsPrincipleExtensions.RetrieveEmailFromPrincipal(user);

            return await input.Users
            .SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}