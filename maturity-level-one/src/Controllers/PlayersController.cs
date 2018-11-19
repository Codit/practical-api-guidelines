using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace maturity_level_one.Controllers
{

    [ApiVersion("1")]
    [Route("world-cup/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidateModel]
    public class PlayersController : ControllerBase
    {
        private readonly IWorldCupRepository _worldCupRepository;

        public PlayersController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        [HttpGet()]
        [SwaggerOperation("get-all-players")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetPlayersAsync([FromQuery(Name = "top-players-only")]bool topPlayersOnly)
        {
            var players = await _worldCupRepository.GetAllPlayersAsync(topPlayersOnly);
            var results = Mapper.Map<IEnumerable<PlayerDto>>(players);
            return Ok(results);
        }

        [HttpPost("{id}/vote")]
        [SwaggerOperation("vote-as-best-player")]
        [SwaggerResponse(202, "Vote accepted")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> VoteAsBestPlayerAsync(int id)
        {
            var player = await _worldCupRepository.GetPlayerAsync(id);
            if (player == null) return NotFound();

            // Voting is not implemented yet, but this kind of api method should return '202' or '200'
            // depending on how it is implemented.

            return Accepted();

        }


        [HttpGet("{id}", Name = "get-player-byid")]
        [SwaggerOperation("get-player-byid")]
        [SwaggerResponse(200, "OK")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> GetPlayerAsync(int id)
        {
            var player = await _worldCupRepository.GetPlayerAsync(id);
            if (player == null) return NotFound();
            var results = Mapper.Map<PlayerDto>(player);

            return Ok(results);

        }

        [HttpPost]
        [SwaggerOperation("create-player")]
        [SwaggerResponse(201, "Created")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> CreateAsync(Player player)
        {
            await _worldCupRepository.CreatePlayerAsync(player);
            return CreatedAtRoute("get-player-byid", new { id = player.Id }, player);
        }

        [HttpPut("{id}")]
        [SwaggerOperation("update-player")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> UpdateFullAsync(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayerAsync(id);
            if (playerObj == null)
            {
                return NotFound();
            }
            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.UpdatePlayerAsync(playerToBeUpdated);
            return NoContent();
        }

        [HttpPatch("{id}")]
        [SwaggerOperation("update-player-incremental")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> UpdateIncrementalAsync(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayerAsync(id);
            if (playerObj == null)
            {
                return NotFound();
            }

            var playerToBeUpdated= Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.ApplyPatchAsync<Player, PlayerDto>(playerToBeUpdated, player);
            return NoContent();
        }

        [HttpPatch("{id}/update")]
        [SwaggerOperation("update-player-jsonpatch")]
        [SwaggerResponse(204, "No Content")]
        [SwaggerResponse(404, "Player not found")]
        [SwaggerResponse(500, "API is not available")]
        public async Task<IActionResult> PatchAsync(int id, [FromBody]JsonPatchDocument<PlayerDto> player)
        {
            // Get our original person object from the DB 
            var playerDb = await _worldCupRepository.GetPlayerAsync(id);
            if (playerDb == null)
            {
                return NotFound();
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