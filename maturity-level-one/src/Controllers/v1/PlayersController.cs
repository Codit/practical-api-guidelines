using System;
using System.Collections.Generic;
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

namespace Codit.LevelOne.Controllers.v1
{

    /// <summary>
    /// All about the football players
    /// </summary>
    [Route("world-cup/v1/[controller]")]
    [ApiController]
    [ValidateModel]
    [SwaggerResponse((int) HttpStatusCode.BadRequest, "Input validation error.")]
    public class PlayersController : ControllerBase
    {
        private const string GetPlayerRoute = "Players_GetPlayer";
        private readonly IWorldCupRepository _worldCupRepository;
        
        public PlayersController(IWorldCupRepository worldCupRepository)
        {
            _worldCupRepository = worldCupRepository;
        }

        /// <summary>
        /// Get players profiles
        /// </summary>
        /// <param name="topPlayersOnly">Indicates whether to return the top players only</param>
        /// <remarks>Provides a profile for all known players</remarks>
        /// <returns>Return a list of Players</returns>
        [HttpGet(Name = "Players_GetPlayers")]
        [SwaggerResponse((int)HttpStatusCode.OK, "List of players", typeof(IEnumerable<PlayerDto>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetPlayers([FromQuery(Name = "top-players-only")]bool topPlayersOnly=false)
        {
            var players = await _worldCupRepository.GetAllPlayersAsync(topPlayersOnly);
            var results = Mapper.Map<IEnumerable<PlayerDto>>(players);
            return Ok(results);
        }

        /// <summary>
        /// Vote as best player
        /// </summary>
        /// <param name="id">Player identifier</param>
        /// <returns>Acknowledge the vote has been accepted</returns>
        [HttpPost("{id}/vote", Name = "Players_VoteAsBestPlayer")]
        [SwaggerResponse((int)HttpStatusCode.Accepted, "Vote has been accepted. No response body")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Player not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> VoteAsBestPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayerAsync(id);
            if (player == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }

            // Voting is not implemented yet, but this kind of api method should return '202' or '200'
            // depending on how it is implemented.

            return Accepted();
        }

        /// <summary>
        /// Get player profile
        /// </summary>
        /// <param name="id">Player identifier</param>
        /// <remarks>Get the profile of a single player</remarks>
        /// <returns>Return a single player</returns>
        [HttpGet("{id}", Name = GetPlayerRoute)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Player data object", typeof(PlayerDto))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Player not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var player = await _worldCupRepository.GetPlayerAsync(id);
            if (player == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }

            var results = Mapper.Map<PlayerDto>(player);
            return Ok(results);

        }

        /// <summary>
        /// Create player
        /// </summary>
        /// <param name="player">Instance of a Player object</param>
        /// <remarks>Add new player to the repository</remarks>
        /// <returns>Acknowledge the object has been created</returns>
        [HttpPost(Name = "Players_Create")]
        [SwaggerResponse((int)HttpStatusCode.Created, "Player has been created. New playes is returned along with the link to the new resource (Location header)")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> Create(NewPlayerDto player)
        {
            var team = await _worldCupRepository.GetTeamAsync(player.TeamId, includePlayers: false);
            if (team == null)
            {
                return BadRequest(new ProblemDetailsError(StatusCodes.Status400BadRequest, $"The Team with Id {player.TeamId} does not exist."));
            }

            if (player.Description == "Evil") throw new ArgumentException("this is evil code");

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

        /// <summary>
        /// Update player profile #1
        /// </summary>
        /// <param name="id">Player identifier</param>
        /// <param name="player">Instance of the player object</param>
        /// <remarks>Update the player profile (full update)</remarks>
        /// <returns>Acknowledge the object has been updated</returns>
        [HttpPut("{id}", Name="Players_UpdateFull")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The data has been updated")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Player not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> UpdateFull(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayerAsync(id);
            if (playerObj == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }

            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.UpdatePlayerAsync(playerToBeUpdated);
            return NoContent();
        }

        /// <summary>
        /// Update player profile #2
        /// </summary>
        /// <param name="id">Player identifier</param>
        /// <param name="player">Instance of the player object</param>
        /// <remarks>Update the player profile (incremental update)</remarks>
        /// <returns>Acknowledge the object has been updated</returns>
        [HttpPatch("{id}", Name = "Players_UpdateIncremental")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The data has been updated")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Player not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> UpdateIncremental(int id, [FromBody] PlayerDto player)
        {
            var playerObj = await _worldCupRepository.GetPlayerAsync(id);
            if (playerObj == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
            }

            var playerToBeUpdated = Mapper.Map<Player>(player);
            playerToBeUpdated.Id = id;

            await _worldCupRepository.ApplyPatchAsync<Player, PlayerDto>(playerToBeUpdated, player);
            return NoContent();
        }

        /// <summary>
        /// Update player profile #3
        /// </summary>
        /// <param name="id">Player identifier</param>
        /// <param name="player">Operation to be performed on the Player in json-patch+json format</param>
        /// <remarks>Update the player profile (incremental update with Json Patch)</remarks>
        /// <returns>Acknowledge the object has been updated</returns>
        [HttpPatch("{id}/update",Name = "Players_UpdateIncrementalJsonPatch")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The data has been updated")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Player not found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "API is not available")]
        public async Task<IActionResult> UpdateIncrementalJsonPatch(int id, [FromBody]JsonPatchDocument<PlayerDto> player)
        {
            // Get our original person object from the DB 
            var playerDb = await _worldCupRepository.GetPlayerAsync(id);
            if (playerDb == null)
            {
                return NotFound(new ProblemDetailsError(StatusCodes.Status404NotFound));
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
