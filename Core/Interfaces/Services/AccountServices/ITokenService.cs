using System.Threading.Tasks;
using Core.Entities.AccountEntities;

namespace Core.Interfaces.Services.AccountServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}