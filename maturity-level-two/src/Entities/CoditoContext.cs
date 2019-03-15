using Microsoft.EntityFrameworkCore;

namespace Codit.LevelTwo.Entities
{
    public class CoditoContext : DbContext
    {

        public CoditoContext(DbContextOptions<CoditoContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Customization> Customizations { get; set; }
    }

}

