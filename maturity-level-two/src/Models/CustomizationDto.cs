using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelTwo.Models
{
    public class CustomizationDto
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }

        public int NumberSold { get; set; }

        public int InventoryLevel { get; set; }

        public int CarId { get; set; }

        
    }
}
