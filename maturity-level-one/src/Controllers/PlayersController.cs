using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace maturity_level_one.Controllers
{

    [ApiVersion("1")]
    [Route("world-cup/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private IWorldCupRepository _worldCupRepository;

        public PlayersController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        [HttpGet()]
        [SwaggerOperation("get-all-players")]
        public async Task<IActionResult> GetPlayers()
        {
            var results = await _worldCupRepository.GetAllPlayers();

            return Ok(results);
        }

        [HttpPost("{id}/vote")]
        [SwaggerOperation("vote-as-best-player")]
        [SwaggerResponse(204, "Accepted")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> VoteAsBestPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayer(id);
            if (player == null) return NotFound();
            
            return NoContent();

        }
    }
}