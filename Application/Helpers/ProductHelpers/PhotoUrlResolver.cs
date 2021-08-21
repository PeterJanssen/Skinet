using Application.Dtos.Product;
using AutoMapper;
using Domain.Models.ProductModels;
using Microsoft.Extensions.Configuration;

namespace Application.Helpers.ProductHelpers
{
    public class PhotoUrlResolver : IValueResolver<Photo, PhotoToReturnDto, string>
    {
        private readonly IConfiguration _config;

        public PhotoUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Photo source, PhotoToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                return _config["ApiUrl"] + source.PictureUrl;
            }

            return null;
        }
    }
}