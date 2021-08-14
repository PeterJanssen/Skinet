using System.IO;
using API.Extensions;
using API.Helpers.SharedHelpers;
using API.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        readonly string _myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCorsPolicy(_myAllowSpecificOrigins);

            services.AddControllers();
            services.AddDatabaseConnections(_config);

            services.AddApplicationServices();
            services.AddIdentityServices(_config);

            services.AddApiVersioningExtension();

            services.AddSwaggerDocumentation();

            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddHealthCheck(_config);

            services.AddMiniProfiler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwaggerDocumentation();
                app.UseMiniProfiler();
            }

            app.UseMiddleware<ExceptionMIddleware>();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Content")
                ),
                RequestPath = "/content"
            });

            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

            app.UseRouting();

            app.UseCors(_myAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(_myAllowSpecificOrigins);
                endpoints.MapFallbackToController("Index", "Fallback").RequireCors(_myAllowSpecificOrigins);
            });
        }
    }
}
