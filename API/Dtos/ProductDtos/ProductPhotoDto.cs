using API.Helpers.SharedHelpers;
using Microsoft.AspNetCore.Http;

namespace API.Dtos.Product
{
    public class ProductPhotoDto
    {
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile Photo { get; set; }
    }
}