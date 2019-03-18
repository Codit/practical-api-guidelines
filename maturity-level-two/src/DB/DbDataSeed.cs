using System;
using System.Collections.Generic;
using System.Linq;
using Codit.LevelTwo.Entities;

namespace Codit.LevelTwo.DB
{
    public static class DbDataSeed
    {
        public static void DataSeed(this CoditoContext context)
        {
            if (context.Cars.Any())
            {
                return;
            }

            var cars = new List<Car>
            {
                new Car
                {
                    Brand = "Volkswagen",
                    Model = "Tiguan",
                    BodyType = CarBodyType.SUV,
                    Description = "Volkswagen's SUV model.",
                    
                    Customizations = new List<Customization>
                    {
                        new Customization
                        {
                            NumberSold = 5,
                            Name = "Tiguan Trendline",
                            Url = "https://fake-url.com",
                            InventoryLevel = 4
                        },
                        new Customization
                        {
                            NumberSold = 3,
                            Name = "Tiguan Comfortline",
                            Url = "https://fake-url.com",
                            InventoryLevel = 4
                        }
                    }
                },
                new Car
                {
                    Brand = "Skoda",
                    Model = "Octavia Combi",
                    BodyType = CarBodyType.Break,
                    Description = "Skoda's most popular break.",

                    Customizations = new List<Customization>
                    {
                        new Customization
                        {
                            NumberSold = 3,
                            Name = "Octavia Combi RS",
                            Url = "https://fake-url.com",
                            InventoryLevel = 8
                        },
                        new Customization
                        {
                            NumberSold = 4,
                            Name = "Octavia Combi Scout",
                            Url = "https://fake-url.com",
                            InventoryLevel = 6
                        }
                    }
                }
            };

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }
    }
}