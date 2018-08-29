using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codit.LevelOne.Entities
{
    public class Player
    {
        public string Description { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Key]
        public int Id { get; set; }

        public bool IsTopPlayer { get; set; }

        [ForeignKey(name: "TeamId")]
        public Team Team { get; set; }

        public int TeamId { get; set; }
    }
}