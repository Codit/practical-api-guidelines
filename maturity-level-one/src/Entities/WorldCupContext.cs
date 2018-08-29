using Microsoft.EntityFrameworkCore;

namespace Codit.LevelOne.Entities
{
    public class WorldCupContext : DbContext
    {
        public WorldCupContext(DbContextOptions<WorldCupContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Team> Teams { get; set; }
    }
}