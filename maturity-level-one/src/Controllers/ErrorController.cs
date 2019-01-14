using Codit.LevelOne.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codit.LevelOne.Controllers
{
    [Route("/errors")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("{code}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ProblemDetails4XX5XX(code));
        }
    }
}