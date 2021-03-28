using API.Dtos.AccountDtos;
using API.Dtos.BasketDtos;
using API.Dtos.OrderDtos;
using API.Dtos.Product;
using API.Helpers.OrderHelpers;
using API.Helpers.ProductHelpers;
using AutoMapper;
using Core.Entities;
using Core.Entities.AccountEntities;
using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;
using Core.Entities.ProductEntities;

namespace API.Helpers.SharedHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>());

            CreateMap<Core.Entities.AccountEntities.Address, AddressDto>().ReverseMap();
            CreateMap<RegisterDto, AppUser>();

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();

            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<AddressDto, Core.Entities.OrderEntities.OrderAddress>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliverMethod, o => o.MapFrom(s => s.DeliverMethod.ShortName))
                .ForMember(d => d.ShippingPrice, o => o.MapFrom(s => s.DeliverMethod.Price));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());

            CreateMap<ProductCreateDto, Product>();

            CreateMap<Review, ProductReviewDto>();

            CreateMap<Photo, PhotoToReturnDto>()
                .ForMember(d => d.PictureUrl, o => o.MapFrom<PhotoUrlResolver>());
        }
    }
}