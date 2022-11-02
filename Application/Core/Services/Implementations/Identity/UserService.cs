using Application.Core.Services.Interfaces.Identity;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Core.Services.Implementations.Identity
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signingManager;

        public UserService(
            ILogger<UserService> logger,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signingManager
            )
        {
            _logger = logger;
            _userManager = userManager;
            _signingManager = signingManager;
        }

        public async Task<AppUser> GetUser(string identifier)
        {
            return await _userManager.Users.Include(appUser => appUser.RefreshTokens).FirstOrDefaultAsync(
                user => user.UserName == identifier ||
                user.Email == identifier
                );
        }
        public async Task<AppUser> GetUserByUserLoginInfo(UserLoginInfo userLoginInfo)
        {
            return await _userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
        }
        public async Task<SignInResult> SignUserIn(AppUser user, string password)
        {
            return await _signingManager.CheckPasswordSignInAsync(user, password, false);
        }
        public async Task<IdentityResult> AddExternalLogin(AppUser user, UserLoginInfo userLoginInfo)
        {
            return await _userManager.AddLoginAsync(user, userLoginInfo);
        }
        public async Task<IList<string>> GetUserRoles(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<IdentityResult> CreateAsync(AppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }
        public async Task<IdentityResult> CreateAsync(AppUser user)
        {
            return await _userManager.CreateAsync(user);
        }
        public async Task<IdentityResult> AddToRoleAsync(AppUser user)
        {
            return await _userManager.AddToRoleAsync(user, UserRoles.Member);
        }
        public async Task<AppUser> FindUserByClaimsPrincipleWithAddressAsync(ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);

            return await _userManager.Users
            .Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Email == email);
        }
        public async Task<IdentityResult> UpdateAsync(AppUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string Member = nameof(Member);
    }
}
