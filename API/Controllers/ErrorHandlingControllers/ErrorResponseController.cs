using Application.Core.Services.Implementations.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Contexts;

namespace API.Controllers.ErrorHandlingControllers
{
    public class ErrorResponseController : BaseApiController
    {
        private readonly StoreContext _storeContext;
        public ErrorResponseController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet("testauth")]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult<string> GetSecretTest()
        {
            return Ok("Secret stuff");
        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {
            var thing = _storeContext.Products.Find(-1);

            if (thing == null) return NotFound;

            return Ok();
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var thing = _storeContext.Products.Find(-1);

            _ = thing.ToString();

            return Ok();
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest;
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }
    }
}