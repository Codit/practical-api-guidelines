using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace Codit.LevelTwo.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        //public string BodyType { get; set; }
        public CarBodyType BodyType { get; set; }

        public string Description { get; set; }

        public ICollection<Customization> Customizations { get; set; } = new List<Customization>();


    }
}