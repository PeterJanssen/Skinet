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

        protected new NotFoundObjectResult NotFound => NotFound(new ApiResponse(404));

        protected new BadRequestObjectResult BadRequest => BadRequest(new ApiResponse(400));

        protected new UnauthorizedObjectResult Unauthorized => Unauthorized(new ApiResponse(401));
    }
}