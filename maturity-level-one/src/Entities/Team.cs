using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codit.LevelOne.Entities
{
    public class Team
    {
        public ContinentCode Continent { get; set; }

        public string Description { get; set; }

        public int FirstAppearence { get; set; }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();

        public int Ranking { get; set; }

        public int TotalWins { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}