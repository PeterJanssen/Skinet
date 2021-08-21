using System.Linq;
using API.Errors;
using Application.Core.Services.Interfaces.OrderServices;
using Application.Core.Services.Interfaces.ProductServices;
using Application.Core.Services.Interfaces.Shared;
using Application.Core.Services.OrderServices;
using Application.Core.Services.ProductServices;
using Application.Core.Services.Shared;
using Infrastructure.Identity.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data.Repository.Implementations;
using Persistence.Data.Repository.Interfaces;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServicesExtension(this IServiceCollection services)
        {
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IDeliveryMethodService, DeliveryMethodService>();
            services.AddScoped<IProductBrandsService, ProductBrandsService>();
            services.AddScoped<IProductTypesService, ProductTypesService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<EmailSender>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPhotoService, PhotoService>();
            services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState
                .Where(ModelStateEntryErrors => ModelStateEntryErrors.Value.Errors.Count > 0)
                .SelectMany(ModelStateEntryErors => ModelStateEntryErors.Value.Errors)
                .Select(ModelError => ModelError.ErrorMessage)
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