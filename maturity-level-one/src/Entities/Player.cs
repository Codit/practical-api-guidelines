using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codit.LevelOne.Entities
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        public int TeamId { get; set; }
    }
}
