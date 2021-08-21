using System.Linq;
using API.Errors;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services.AccountServices;
using Core.Interfaces.Services.OrderServices;
using Core.Interfaces.Services.ProductServices;
using Core.Interfaces.Services.Shared;
using Infrastructure.Data.Repositories;
using Infrastructure.Identity.Email;
using Infrastructure.Services.AccountServices;
using Infrastructure.Services.OrderServices;
using Infrastructure.Services.ProductServices;
using Infrastructure.Services.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IDeliveryMethodService, DeliveryMethodService>();
            services.AddScoped<IProductBrandsService, ProductBrandsService>();
            services.AddScoped<IProductTypesService, ProductTypesService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<EmailSender>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPhotoService, PhotoService>();
            services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState
                .Where(errors => errors.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToArray();

                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });

            return services;
        }
    }
}