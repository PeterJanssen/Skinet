using Application.Dtos.OrderDtos;
using AutoMapper;
using Domain.Models.OrderModels;
using Microsoft.Extensions.Configuration;

namespace Application.Helpers.OrderHelpers
{
    public class OrderItemUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration _config;
        public OrderItemUrlResolver(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ItemOrdered.PictureUrl))
            {
                return _config["ApiUrl"] + source.ItemOrdered.PictureUrl;
            }

            return null;
        }
    }
}