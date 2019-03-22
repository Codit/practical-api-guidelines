using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codit.LevelTwo.Entities;
using Codit.LevelTwo.Services;
using Microsoft.AspNetCore.Mvc;

namespace Codit.UnitTest
{
    internal class CoditoRepositoryFake : ICoditoRepository
    {
        private readonly List<Car> _cars;

        public CoditoRepositoryFake()
        {
            _cars = new List<Car>
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
            _cars[0].Customizations.Add(new Customization
            {
                NumberSold = 5,
                Name = "Tiguan Trendline",
                Url = "https://fake-url.com",
                InventoryLevel = 4
            });
            _cars[0].Customizations.Add(new Customization
            {
                NumberSold = 3,
                Name = "Tiguan Comfortline",
                Url = "https://fake-url.com",
                InventoryLevel = 4
            });
            _cars[1].Customizations.Add(new Customization
            {
                NumberSold = 3,
                Name = "Octavia Combi RS",
                Url = "https://fake-url.com",
                InventoryLevel = 8
            });
            _cars[1].Customizations.Add(new Customization
            {
                NumberSold = 4,
                Name = "Octavia Combi Scout",
                Url = "https://fake-url.com",
                InventoryLevel = 6
            });
        }

        public async Task<SalesRequestResult> ApplyCustomizationSaleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task ApplyPatchAsync<TEntity>(TEntity entityUpdated) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> CarExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task CreateCustomizationAsync(Customization customization)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CustomizationExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteCustomizationAsync(int id)

        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customization>> GetAllCustomizationsAsync()
        {
            // BodyType query not tested!
            return Task.FromResult((IEnumerable<Customization>)_cars[0].Customizations);
        }

        public Task<Car> GetCarAsync(int id, bool includeCustomization)
        {
            var filtered = _cars.Where(c => (c.Id == id)).ToArray();
            Car car = filtered[0];
            return Task.FromResult(car);
        }

        public Task<IEnumerable<Car>> GetCarsAsync(CarBodyType? bodyType)
        {
            return Task.FromResult<IEnumerable<Car>>(_cars);
            
        }

        public Task<Customization> GetCustomizationAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
