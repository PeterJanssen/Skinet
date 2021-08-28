using Application.Core.Services.Interfaces.LoggerServices;
using Application.Core.Specifications.LogSpec;
using Domain.Models.LogModels;
using Persistence.Data.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Implementations.LogServices
{
    public class LogService : ILogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<Log>> GetLogsAsync(LogSpecParams specParams)
        {
            var specification = new LogSpecification(specParams);

            var books = await _unitOfWork.Repository<Log>().ListAsync(specification);

            return books;
        }
        public async Task<int> CountLogsAsync(LogSpecParams specParams)
        {
            var countSpec = new LogWithFilterForCountSpecification(specParams);

            var totalItems = await _unitOfWork.Repository<Log>().CountAsync(countSpec);

            return totalItems;
        }
    }
}