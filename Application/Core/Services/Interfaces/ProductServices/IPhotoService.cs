using System.Threading.Tasks;
using Domain.Models.ProductModels;
using Microsoft.AspNetCore.Http;

namespace Application.Core.Services.Interfaces.ProductServices
{
    public interface IPhotoService
    {
        Task<Photo> SaveToDiskAsync(IFormFile photo);
        void DeleteFromDisk(Photo photo);
    }
}