using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Extensions;
using Codit.LevelOne.Models;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace maturity_level_one.Controllers
{

    [ApiVersion("1")]
    [Route("world-cup/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidateModel]
    public class PlayersController : ControllerBase
    {
        private IWorldCupRepository _worldCupRepository;

        public PlayersController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        [HttpGet()]
        [SwaggerOperation("get-all-players")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetPlayers([FromQuery(Name = "top-players-only")]bool topPlayersOnly)
        {
            var players = await _worldCupRepository.GetAllPlayers(topPlayersOnly);
            var results = Mapper.Map<IEnumerable<PlayerDto>>(players);
            return Ok(results);
        }

        [HttpPost("{id}/vote")]
        [SwaggerOperation("vote-as-best-player")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> VoteAsBestPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayer(id);
            if (player == null) return NotFound();
            
            return NoContent();

        }


        [HttpGet("{id}", Name = "get-player-byid")]
        [SwaggerOperation("get-player-byid")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayer(id);
            if (player == null) return NotFound();
            var results = Mapper.Map<PlayerDto>(player);

            return Ok(results);

        }

        [HttpPost]
        [SwaggerOperation("create-player")]
        [SwaggerResponse(201, "Created")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> Create(NewPlayerDto player)
        {
            var team = await _worldCupRepository.GetTeamAsync(player.TeamId, includePlayers: false);

            if (team == null)
            {
                return BadRequest($"The Team with Id {player.TeamId} does not exist.");
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

            return CreatedAtRoute("get-player-byid", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation("update-player")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> UpdateFull(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayer(id);
            if (playerObj == null)
            {
                return NotFound();
            }
            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.UpdatePlayer(playerToBeUpdated);
            return NoContent();
        }

        [HttpPatch("{id}")]
        [SwaggerOperation("update-player-incremental")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> UpdateIncremental(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayer(id);
            if (playerObj == null)
            {
                return NotFound();
            }

            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.ApplyPatch<Player, PlayerDto>(playerToBeUpdated, player);
            return NoContent();
        }

        [HttpPatch("{id}/update")]
        [SwaggerOperation("update-player-jsonpatch")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> Patch(int id, [FromBody]JsonPatchDocument<PlayerDto> player)
        {
            // Get our original person object from the DB 
            var playerDb = await _worldCupRepository.GetPlayer(id);
            if (playerDb == null)
            {
                return NotFound();
            }
            // DB to DTO
            var playerToBeUpdated = Mapper.Map<PlayerDto>(playerDb);
            //Apply the patch to the DTO. 
            player.ApplyTo(playerToBeUpdated);
            Mapper.Map(playerToBeUpdated, playerDb);
            await _worldCupRepository.UpdatePlayer(playerDb);


            var results = Mapper.Map<PlayerDto>(playerDb);
            return Ok(results);
        }

    }
}