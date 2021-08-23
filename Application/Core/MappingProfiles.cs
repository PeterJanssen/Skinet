using Application.Dtos.AccountDtos;
using Application.Dtos.BasketDtos;
using Application.Dtos.OrderDtos;
using Application.Dtos.ProductDtos;
using Application.Helpers.OrderHelpers;
using Application.Helpers.ProductHelpers;
using AutoMapper;
using Domain.Models.AccountModels.AppUserModels;
using Domain.Models.BasketModels;
using Domain.Models.OrderModels;
using Domain.Models.ProductModels;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>());

            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<RegisterRequest, AppUser>();

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<AddressDto, OrderAddress>();
            CreateMap<OrderAddressDto, OrderAddress>().ReverseMap();

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

            CreateMap<ProductTypeDto, ProductType>().ReverseMap();

            CreateMap<ProductBrandDto, ProductBrand>().ReverseMap();

            CreateMap<DeliveryMethodDto, DeliveryMethod>().ReverseMap();
        }
    }
}