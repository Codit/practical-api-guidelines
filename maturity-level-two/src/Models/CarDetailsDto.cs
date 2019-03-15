using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelTwo.Models
{
    public class CarDetailsDto
    {
        public int Id { get; set; }
        public string Brand { get; set; }

        public string Model { get; set; }

        public ICollection<CustomizationDto> Customizations { get; set; }
                = new List<CustomizationDto>();
    }
}
