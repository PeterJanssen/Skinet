using Application.Core.Specifications.LogSpec;
using Domain.Models.LogModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.LoggerServices
{
    public interface ILogService
    {
        Task<IReadOnlyList<Log>> GetLogsAsync(LogSpecParams specParams);
        Task<int> CountLogsAsync(LogSpecParams specParams);
    }
}