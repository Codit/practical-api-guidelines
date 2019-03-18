using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Codit.LevelTwo.Entities;

namespace Codit.LevelTwo.Models
{
    public class CarDto
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public CarBodyType BodyType { get; set; }

        public string Description { get; set; }

    }
}
