using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace maturity_level_one.Controllers
{

    [ApiVersion("1")]
    [Route("world-cup/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
    }
}