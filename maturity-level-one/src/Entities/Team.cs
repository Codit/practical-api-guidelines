using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Codit.LevelOne.Entities
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Ranking { get; set; }

        public int FirstAppearence { get; set; }

        public int TotalWins { get; set; }

        public DateTime UpdatedOn { get; set; }
        public ICollection<Player> Players { get; set; }
               = new List<Player>();
    }
}

