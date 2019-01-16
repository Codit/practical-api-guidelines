using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Codit.LevelOne.Controllers
{
    [Route("world-cup/v1/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        ///     Get Health
        /// </summary>
        /// <remarks>Provides an indication about the health of the runtime</remarks>
        [HttpGet(Name = "Health_Get")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Runtime is up and running in a healthy state")]
        [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable, "Runtime is not healthy")]

        public IActionResult Get()
        {
            return Ok();
        }

    }
}