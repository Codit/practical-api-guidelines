using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codit.LevelOne.Entities
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Description { get; set; }
        public bool IsTopPlayer { get; set; }
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        public int TeamId { get; set; }
    }
}
