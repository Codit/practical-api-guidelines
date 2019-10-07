using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Codit.LevelOne.Extensions;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Codit.LevelOne.Controllers.v1
{
    /// <summary>
    /// All about the teams qualified to the World Cup
    /// </summary>
    [Route("world-cup/v1/[controller]")]
    [ApiController]
    [ValidateModel]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Input validation error.")]
    public class TeamsController : ControllerBase
    {
        private readonly IWorldCupRepository _worldCupRepository;

        public TeamsController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
            AutoMapperConfig.Initialize();
        }


        /// <summary>
        /// Get Teams
        /// </summary>
        /// <remarks>Provides the details for all known team</remarks>
        /// <returns>Return a list of Team</returns>
        [HttpGet(Name = "Teams_GetTeams")]
        [SwaggerResponse((int)HttpStatusCode.OK, "List of teams", typeof(IEnumerable<TeamDto>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetTeams()
        {

            var teams = await _worldCupRepository.GetTeamsAsync();
            var results = Mapper.Map<IEnumerable<TeamDto>>(teams);

            return Ok(results);
        }

        /// <summary>
        /// Get Team
        /// </summary>
        /// <remarks>Provides the details of a single team</remarks>
        /// <returns>Return a Team instance</returns>
        [HttpGet("{id}", Name = "Teams_GetTeam")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Team data")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Team not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _worldCupRepository.GetTeamAsync(id, true);
            if (team == null) return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));

            var teamResult = Mapper.Map<TeamDetailsDto>(team);
            return Ok(teamResult);
        }
    }
}