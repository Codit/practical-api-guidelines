using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Extensions;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Codit.LevelOne.Controllers
{

    [Route("world-cup/v1/[controller]")]
    [ApiController]
    [ValidateModel]
    public class PlayersController : ControllerBase
    {
        private const string GetPlayerRoute = "Players_GetPlayer";
        private readonly IWorldCupRepository _worldCupRepository;

        public PlayersController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        [HttpGet(Name = "Players_GetPlayers")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetPlayers([FromQuery(Name = "top-players-only")]bool topPlayersOnly)
        {
            var players = await _worldCupRepository.GetAllPlayersAsync(topPlayersOnly);
            var results = Mapper.Map<IEnumerable<PlayerDto>>(players);
            return Ok(results);
        }

        [HttpPost("{id}/vote", Name = "Players_VoteAsBestPlayer")]
        [SwaggerResponse(202, "Vote accepted")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> VoteAsBestPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayerAsync(id);
            if (player == null)
            {
                return NotFound(new ProblemDetails4XX5XX(StatusCodes.Status404NotFound));
            }

            // Voting is not implemented yet, but this kind of api method should return '202' or '200'
            // depending on how it is implemented.

            return Accepted();
        }
        
        [HttpGet("{id}", Name = GetPlayerRoute)]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayerAsync(id);
            if (player == null)
            {
                return NotFound(new ProblemDetails4XX5XX(StatusCodes.Status404NotFound));
            }

            var results = Mapper.Map<PlayerDto>(player);
            return Ok(results);

        }

        [HttpPost(Name = "Players_Create")]
        [SwaggerResponse(201, "Created")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> Create(NewPlayerDto player)
        {
            var team = await _worldCupRepository.GetTeamAsync(player.TeamId, includePlayers: false);
            if (team == null)
            {
                return BadRequest(new ProblemDetails4XX5XX(StatusCodes.Status400BadRequest, $"The Team with Id {player.TeamId} does not exist."));
            }

            var playerEntity = new Player
            {
                FirstName = player.FirstName,
                Description = player.Description,
                IsTopPlayer = player.IsTopPlayer,
                TeamId = player.TeamId
            };

            await _worldCupRepository.CreatePlayerAsync(playerEntity);
            var result = Mapper.Map<PlayerDto>(playerEntity);

            return CreatedAtRoute(GetPlayerRoute, new { id = result.Id }, result);
        }

        [HttpPut("{id}", Name="Players_UpdateFull")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> UpdateFull(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayerAsync(id);
            if (playerObj == null)
            {
                return NotFound(new ProblemDetails4XX5XX(StatusCodes.Status404NotFound));
            }

            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.UpdatePlayerAsync(playerToBeUpdated);
            return NoContent();
        }

        [HttpPatch("{id}", Name = "Players_UpdateIncremental")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> UpdateIncremental(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayerAsync(id);
            if (playerObj == null)
            {
                return NotFound(new ProblemDetails4XX5XX(StatusCodes.Status404NotFound));
            }

            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.ApplyPatchAsync<Player, PlayerDto>(playerToBeUpdated, player);
            return NoContent();
        }

        [HttpPatch("{id}/update",Name = "Players_UpdateIncrementalJsonPatch")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> Patch(int id, [FromBody]JsonPatchDocument<PlayerDto> player)
        {
            // Get our original person object from the DB 
            var playerDb = await _worldCupRepository.GetPlayerAsync(id);
            if (playerDb == null)
            {
                return NotFound(new ProblemDetails4XX5XX(StatusCodes.Status404NotFound));
            }

            // DB to DTO
            var playerToBeUpdated = Mapper.Map<PlayerDto>(playerDb);

            //Apply the patch to the DTO. 
            player.ApplyTo(playerToBeUpdated);
            Mapper.Map(playerToBeUpdated, playerDb);
            await _worldCupRepository.UpdatePlayerAsync(playerDb);

            var results = Mapper.Map<PlayerDto>(playerDb);
            return Ok(results);
        }

    }
}
