using Codit.LevelOne.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelOne.DB
{
    public static class DbDataSeed
    {
        public static void DataSeed(this WorldCupContext context)
        {
            if (context.Teams.Any())
            {
                return;
            }

            var teams = new List<Team>()
            {
                new Team()
                {
                     Name = "Belgium",
                     Description = "The one with that big park.",
                     UpdatedOn = DateTime.MinValue,
                     Players = new List<Player>()
                     {
                         new Player() {
                             FirstName = "Hazard",
                             Description = "He plays in Chelsea."
                         },
                          new Player() {
                             FirstName = "De Bruyne",
                             Description = "He scored the last match."
                          },
                     }
                },
                new Team()
                {
                    Name = "France",
                    Description = "One time world cup winner.",
                    UpdatedOn = DateTime.UtcNow,
                    Players = new List<Player>()
                     {
                         new Player() {
                             FirstName = "MBappe",
                             Description = "19 years old striker."
                         },
                          new Player() {
                             FirstName = "Pogba",
                             Description = "He plays for MUTD."
                          },
                     }
                }
            };

            context.Teams.AddRange(teams);
            context.SaveChanges();  
        }

    }
}
