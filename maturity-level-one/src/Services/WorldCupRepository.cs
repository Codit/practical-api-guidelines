using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codit.LevelOne.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Codit.LevelOne.Extensions;

namespace Codit.LevelOne.Services
{
    public class WorldCupRepository : IWorldCupRepository
    {
        private WorldCupContext _context;

        public WorldCupRepository(WorldCupContext context)
        {
            _context = context;
            //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<Player> GetPlayerOfTeam(int teamId, int playerId)
        {
            return await _context.Players.Where(p => p.TeamId == teamId && p.Id == playerId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Player>> GetPlayersOfTeam(int teamId)
        {
            return await _context.Players.Where(t => t.TeamId == teamId).ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetAllPlayers(bool topPlayersOnly)
        {
            if (topPlayersOnly)
            {
                return await _context.Players.Where(p => p.IsTopPlayer == topPlayersOnly).ToListAsync();
            }
            return await _context.Players.ToListAsync();
        }
        public async Task<Team> GetTeam(int teamId, bool includePlayers)
        {
            if (includePlayers)
            {
                return await _context.Teams.Include(t => t.Players)
                    .Where(p => p.Id == teamId).FirstOrDefaultAsync();
            }

            return await _context.Teams.Where(t => t.Id == teamId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Team>> GetTeams()
        {
            return await _context.Teams.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            return await _context.Players.AsNoTracking().Where(p => p.Id == playerId).FirstOrDefaultAsync();
        }

        public async Task<bool> Save()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<bool> TeamExists(int teamId)
        {
            return await _context.Teams.AnyAsync(t => t.Id == teamId);
        }

        public async Task CreatePlayer(Player player)
        {

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

        }

        public async Task UpdatePlayer(Player player)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();

        }

        public async Task ApplyPatch<TEntity, TDto>(TEntity entityToUpdate, TDto dto) where TEntity : class
        {
            if (dto == null)
                throw new ArgumentNullException($"{nameof(dto)}", $"{nameof(dto)} cannot be null.");

            var properties = dto.GetFilledProperties();
            _context.Attach(entityToUpdate);

            foreach (var property in properties)
            {
                _context.Entry(entityToUpdate).Property(property).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
