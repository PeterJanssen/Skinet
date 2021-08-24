using API.Errors;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMapper _mapper;

        protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();

        protected NotFoundObjectResult NotFound(string message = null) => NotFound(new ApiResponse(404, message));

        protected BadRequestObjectResult BadRequest(string message = null) => BadRequest(new ApiResponse(400, message));

        protected UnauthorizedObjectResult Unauthorized(string message = null) => Unauthorized(new ApiResponse(401, message));
    }
}