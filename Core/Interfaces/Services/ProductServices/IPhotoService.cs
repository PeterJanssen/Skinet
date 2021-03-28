using System.Threading.Tasks;
using Core.Entities.ProductEntities;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services.ProductServices
{
    public interface IPhotoService
    {
        Task<Photo> SaveToDiskAsync(IFormFile photo);
        void DeleteFromDisk(Photo photo);
    }
}