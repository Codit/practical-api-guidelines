using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelOne.Entities
{
    public class WorldCupContext : DbContext
    {
        public WorldCupContext(DbContextOptions<WorldCupContext> options): base(options)
        {
            //create DB
            //Database.EnsureCreated();  
            //Database.Migrate();  
        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
