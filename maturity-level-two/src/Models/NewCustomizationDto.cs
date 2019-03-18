using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelTwo.Models
{
    public class NewCustomizationDto
    {

        public string Name { get; set; }

        public string Url { get; set; }

        public int CarId { get; set; }

        public int InventoryLevel { get; set; }

        //the new customization has not been sold yet.

    }
}
