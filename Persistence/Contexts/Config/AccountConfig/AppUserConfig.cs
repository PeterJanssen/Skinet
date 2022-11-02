using Domain.Models.AccountModels.AppUserModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Contexts.Config.AccountConfig
{
    public class AppUserConfig
    {
        public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
        {
            public void Configure(EntityTypeBuilder<AppUser> builder)
            {
                builder.HasMany(appUser => appUser.RefreshTokens)
                .WithOne(refreshToken => refreshToken.AppUser)
                .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}