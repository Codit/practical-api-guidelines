using Codit.LevelOne.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelOne.Services
{
    public interface IWorldCupRepository
    {
        Task<bool> TeamExistsAsync(int teamId);
        Task<IEnumerable<Team>> GetTeamsAsync();
        Task<Team> GetTeamAsync(int teamId, bool includePlayers);
        Task<IEnumerable<Player>> GetPlayersOfTeamAsync(int teamId);
        Task<Player> GetPlayerOfTeamAsync(int teamId, int playerId);
        Task<IEnumerable<Player>> GetAllPlayersAsync(bool topPlayersOnly);
        Task<Player> GetPlayerAsync(int playerId);
        Task<bool> SaveAsync();
        Task CreatePlayerAsync(Player player);
        Task UpdatePlayerAsync(Player player);
        Task ApplyPatchAsync<TEntity,TDto>(TEntity entityName, TDto dto) where TEntity : class;
    }
}

