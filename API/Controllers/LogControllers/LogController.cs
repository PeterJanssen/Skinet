using System.Threading.Tasks;
using Application.Core.Paging;
using Application.Core.Services.Implementations.Identity;
using Application.Core.Services.Interfaces.LoggerServices;
using Application.Core.Specifications.LogSpec;
using Domain.Models.LogModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.LogControllers
{

    [Produces("application/json")]
    [Authorize(Roles = UserRoles.Admin)]
    public class LogController : BaseApiController
    {
        public ILogService _logService;
        public LogController(ILogService logService)
        {
            _logService = logService;
        }


        /// <summary>
        /// Gets logs from the database
        /// </summary>
        /// <response code="200">Returns the product with the provided id</response>
        /// <response code="401">Returns if current user is not logged in</response>
        /// <response code="403">Returns if current user is not an admin</response>
        [HttpGet]
        public async Task<IActionResult> GetAllLogsFromDB([FromQuery] LogSpecParams specParams)
        {
            var data = await _logService.GetLogsAsync(specParams);

            var totalItems = await _logService.CountLogsAsync(specParams);

            return Ok(new Pagination<Log>(specParams.PageIndex, specParams.PageSize, totalItems, data));

        }
    }
}