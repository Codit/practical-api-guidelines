using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Codit.LevelOne.Extensions;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Codit.LevelOne.Controllers
{
    [Route("world-cup/v1/[controller]")]
    [ApiController]
    [ValidateModel]
    public class TeamsController : ControllerBase
    {
        private readonly IWorldCupRepository _worldCupRepository;

        public TeamsController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        [HttpGet(Name = "Teams_GetTeams")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetTeams()
        {

            var teams = await _worldCupRepository.GetTeamsAsync();
            var results = Mapper.Map<IEnumerable<TeamDto>>(teams);

            return Ok(results);
        }

        [HttpGet("{id}", Name = "Teams_GetTeam")]
        [SwaggerResponse(200, "Accepted")]
        [SwaggerResponse(404, "Team not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _worldCupRepository.GetTeamAsync(id, true);
            if (team == null) return NotFound(new ProblemDetails4XX5XX(StatusCodes.Status404NotFound));

            var teamResult = Mapper.Map<TeamDetailsDto>(team);
            return Ok(teamResult);
        }
    }
}