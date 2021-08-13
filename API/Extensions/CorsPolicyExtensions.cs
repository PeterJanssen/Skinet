using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class CorsPolicyExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string specificOrigins)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy(specificOrigins, policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("https://localhost:4200");
                });
            });

            return services;
        }
    }
}