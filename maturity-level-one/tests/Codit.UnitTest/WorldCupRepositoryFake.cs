using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Services;

namespace Codit.UnitTest
{
    internal class WorldCupRepositoryFake : IWorldCupRepository
    {
        private readonly List<Team>  _teams;

        public WorldCupRepositoryFake()
        {
            _teams = new List<Team>
            {
                new Team
                {
                    Name = "Belgium",
                    Description = "The one with that big park.",
                    UpdatedOn = DateTime.Now.AddDays(value: -1),
                    Continent = ContinentCode.Europe,
                    Players = new List<Player>
                    {
                        new Player
                        {
                            FirstName = "Hazard",
                            Description = "He plays in Chelsea.",
                            IsTopPlayer = true
                        },
                        new Player
                        {
                            FirstName = "De Bruyne",
                            Description = "He scored the last match."
                        }
                    }
                },
                new Team
                {
                    Name = "France",
                    Description = "One time world cup winner.",
                    Continent = ContinentCode.Europe,
                    UpdatedOn = DateTime.Now,
                    Players = new List<Player>
                    {
                        new Player
                        {
                            FirstName = "MBappe",
                            Description = "19 years old striker."
                        },
                        new Player
                        {
                            FirstName = "Pogba",
                            Description = "He plays for MUTD.",
                            IsTopPlayer = true
                        }
                    }
                }
            };
        }
        public Task ApplyPatchAsync<TEntity, TDto>(TEntity entityName, TDto dto) where TEntity : class
        {
            throw new System.NotImplementedException();
        }

        public Task CreatePlayerAsync(Player player)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Player>> GetAllPlayersAsync(bool topPlayersOnly)
        {
            throw new System.NotImplementedException();
        }

        public Task<Player> GetPlayerAsync(int playerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Player> GetPlayerOfTeamAsync(int teamId, int playerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Player>> GetPlayersOfTeamAsync(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Team> GetTeamAsync(int teamId, bool includePlayers)
        {
            return null;
        }

        public Task<IEnumerable<Team>> GetTeamsAsync()
        {
            return Task.FromResult<IEnumerable<Team>>(_teams);
        }

        public Task<bool> SaveAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> TeamExistsAsync(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdatePlayerAsync(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}