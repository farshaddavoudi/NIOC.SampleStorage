using Bit.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NIOC.SampleStorage.Server.Api.Extensions;

namespace NIOC.SampleStorage.Server.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        public string? CurrentUserDomainName
        {
            get
            {
                if (HttpContext.User.Identity!.IsAuthenticated is false)
                    throw new UnauthorizedException(); // User Is Not Authenticated

                return User.Identity!.GetUserDomainName();
            }
        }

        /// <summary>
        /// Status201: Created 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult Ok201(object value)
        {
            return StatusCode(StatusCodes.Status201Created, value);
        }

        /// <summary>
        /// Status201: Created
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public StatusCodeResult Ok201()
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Status204: NoContent
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public NoContentResult Ok204()
        {
            return NoContent();
        }
    }
}
