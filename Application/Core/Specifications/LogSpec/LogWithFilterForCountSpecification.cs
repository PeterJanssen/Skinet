using Domain.Models.LogModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.LogSpec
{
    public class LogWithFilterForCountSpecification : Specification<Log>
    {
        public LogWithFilterForCountSpecification(LogSpecParams specParams)
            : base(log =>
            (string.IsNullOrEmpty(specParams.Search) ||
                (log.Message.ToLower().Contains(specParams.Search) ||
                 log.Exception.ToLower().Contains(specParams.Search))
            ) &&
                (string.IsNullOrEmpty(specParams.Level) || log.Level.ToLower().Contains(specParams.Level))
            )
        {

        }
    }
}