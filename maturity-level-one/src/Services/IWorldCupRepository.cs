using Codit.LevelOne.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelOne.Services
{
    public interface IWorldCupRepository
    {
        Task<bool> TeamExists(int teamId);
        Task<IEnumerable<Team>> GetTeams();
        Task<Team> GetTeam(int teamId, bool includePlayers);
        Task<IEnumerable<Player>> GetPlayersOfTeam(int teamId);
        Task<Player> GetPlayerOfTeam(int teamId, int playerId);
        Task<IEnumerable<Player>> GetAllPlayers();
        Task<Player> GetPlayer(int playerId);
        Task<bool> Save();

    }
}
