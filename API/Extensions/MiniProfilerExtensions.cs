using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class MiniProfilerExtensions
    {
        public static IServiceCollection AddMiniProfilerExtension(this IServiceCollection services)
        {
            services.AddMiniProfiler(options =>
            {
                options.ColorScheme = StackExchange.Profiling.ColorScheme.Dark;
            }).AddEntityFramework();

            return services;
        }
    }
}