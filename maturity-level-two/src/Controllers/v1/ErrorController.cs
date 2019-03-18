using Codit.LevelTwo.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codit.LevelTwo.Controllers.v1
{
    [Route("/errors")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Manages the unmatched routes
        /// </summary>
        /// <param name="code">HTTP status code</param>
        /// <returns>Error formatted as application/problem+json</returns>
        [Route("{code}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ProblemDetailsError(code));
        }
    }
}