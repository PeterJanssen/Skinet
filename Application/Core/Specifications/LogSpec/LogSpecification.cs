using Domain.Models.LogModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.LogSpec
{
    public class LogSpecification : Specification<Log>
    {
        public LogSpecification(LogSpecParams specParams)
            : base(log =>
            (string.IsNullOrEmpty(specParams.Search) ||
                (log.Message.ToLower().Contains(specParams.Search) ||
                 log.Exception.ToLower().Contains(specParams.Search))
            ) &&
                (string.IsNullOrEmpty(specParams.Level) || log.Level.ToLower().Contains(specParams.Level))
            )
        {
            AddOrderBy(log => log.TimeStamp);
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "TimeStampAsc":
                        AddOrderBy(log => log.TimeStamp);
                        break;
                    case "TimeStampDesc":
                        AddOrderByDescending(log => log.TimeStamp);
                        break;
                    default:
                        AddOrderByDescending(log => log.TimeStamp);
                        break;
                }
            }
        }
    }
}