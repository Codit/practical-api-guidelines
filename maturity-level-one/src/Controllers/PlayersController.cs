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
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _worldCupRepository.GetAllPlayers();
            var results = Mapper.Map<IEnumerable<PlayerDto>>(players);
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
        public async Task<IActionResult> Create(Player player)
        {
            await _worldCupRepository.CreatePlayer(player);
            return CreatedAtRoute("get-player-byid", new { id = player.Id }, player);
        }

    }
}