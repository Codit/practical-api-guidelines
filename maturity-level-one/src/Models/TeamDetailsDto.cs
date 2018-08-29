using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelOne.Models
{
    public class TeamDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PlayerDto> Players { get; set; }
                = new List<PlayerDto>();
    }
}

