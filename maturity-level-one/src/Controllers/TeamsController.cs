using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Codit.LevelOne.Extensions;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace maturity_level_one.Controllers
{
    [ApiVersion("1")]
    [Route("world-cup/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidateModel]
    public class TeamsController : ControllerBase
    {
        private IWorldCupRepository _worldCupRepository;

        public TeamsController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        [HttpGet()]
        [SwaggerOperation("get-teams")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetTeams()
        {

            var teams = await _worldCupRepository.GetTeams();
            var results = Mapper.Map<IEnumerable<TeamDto>>(teams);

            return Ok(results);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("get-team-by-id")]
        [SwaggerResponse(200, "Accepted")]
        [SwaggerResponse(404, "Team not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _worldCupRepository.GetTeam(id, true);
            if (team == null) return NotFound();

            var teamResult = Mapper.Map<TeamDetailsDto>(team);
            return Ok(teamResult);

        }
    }
}