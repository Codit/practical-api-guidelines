using System.ComponentModel.DataAnnotations;

namespace Codit.LevelOne.Models
{
    public class NewPlayerDto
    {
        [Required]
        public string FirstName { get; set; }
        public string Description { get; set; }
        public bool IsTopPlayer { get; set; }
        [Required]
        public int TeamId { get; set; }
    }
}