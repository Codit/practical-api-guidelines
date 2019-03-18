using Codit.LevelTwo.Entities;
using System.Collections.Generic;
using System.Linq;


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
                    Description = "Volkswagen's SUV model."
                },
                new Car
                {
                    Brand = "Skoda",
                    Model = "Octavia Combi",
                    BodyType = CarBodyType.Break,
                    Description = "Skoda's most popular break."
                }
            };
            cars[0].Customizations.Add(new Customization
            {
                NumberSold = 5,
                Name = "Tiguan Trendline",
                Url = "https://fake-url.com",
                InventoryLevel = 4
            });
            cars[0].Customizations.Add(new Customization
            {
                NumberSold = 3,
                Name = "Tiguan Comfortline",
                Url = "https://fake-url.com",
                InventoryLevel = 4
            });
            cars[1].Customizations.Add(new Customization
            {
                NumberSold = 3,
                Name = "Octavia Combi RS",
                Url = "https://fake-url.com",
                InventoryLevel = 8
            });
            cars[1].Customizations.Add(new Customization
            {
                NumberSold = 4,
                Name = "Octavia Combi Scout",
                Url = "https://fake-url.com",
                InventoryLevel = 6
            });

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }
    }
}