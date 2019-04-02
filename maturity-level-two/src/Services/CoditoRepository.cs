using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Threading.Tasks;

using Codit.LevelTwo.Entities;
using Codit.LevelTwo.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Codit.LevelTwo.Services
{
    public class CoditoRepository : ICoditepository
    {
        private readonly CoditoContext _coditoContext;

        public CoditoRepository(CoditoContext coditoContext)
        {
            _coditoContext = coditoContext;
        }

        public async Task<bool> CarExistsAsync(int id)
        {
            return await _coditoContext.Cars.AnyAsync(car => car.Id == id);
        }

        public async Task<bool> CustomizationExistsAsync(int id)
        {
            return await _coditoContext.Customizations.AnyAsync(customization => customization.Id == id);
        }

        public async Task<IEnumerable<Car>> GetCarsAsync(CarBodyType? bodyType)
        {
            if (bodyType == null)
            {
                return await _coditoContext.Cars.OrderBy(car => car.Id).ToListAsync();
            }
            return await _coditoContext.Cars.Where(car => car.BodyType == bodyType).OrderBy(car => car.Id).ToListAsync();
        }

        public async Task<Car> GetCarAsync(int id, bool includeCustomization)
        {
            if (includeCustomization)
            {
                return await _coditoContext.Cars.Include(car => car.Customizations)
                    .Where(car => car.Id == id).FirstOrDefaultAsync();
            }

            return await _coditoContext.Cars.Where(car => car.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Customization>> GetAllCustomizationsAsync()
        {
           return await _coditoContext.Customizations.OrderByDescending(cust => cust.NumberSold).ToListAsync();
        }

        public async Task<Customization> GetCustomizationAsync(int id)
        {
            return await _coditoContext.Customizations.AsNoTracking().Where(cust => cust.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateCustomizationAsync(Customization customization)
        {

             _coditoContext.Customizations. AddAsync(customization);
            await _coditoContext.SaveChangesAsync();
            
        }

        public async Task ApplyPatchAsync<TEntity>(TEntity entityUpdated) where TEntity : class
        {
            if (entityUpdated == null)
                throw new ArgumentNullException($"{nameof(entityUpdated)}", $"{nameof(entityUpdated)} cannot be null.");

            var properties = entityUpdated.GetFilledProperties();
            _coditoContext.Attach(entityUpdated);

            foreach (var property in properties)
            {
                if(!_coditoContext.Entry(entityUpdated).Property(property).Metadata.IsKey())
                {
                    _coditoContext.Entry(entityUpdated).Property(property).IsModified = true;
                }               
            }

            await _coditoContext.SaveChangesAsync();
        }

        public async Task<int> DeleteCustomizationAsync(int id)
        {

            bool exists = await CustomizationExistsAsync(id);

            if (exists)
            {
                var entityToDelete = new Customization();
                Type type = entityToDelete.GetType();
                PropertyInfo prop = type.GetProperty("Id");
                prop.SetValue(entityToDelete, id, null);
                _coditoContext.Customizations.Attach(entityToDelete);
                _coditoContext.Customizations.Remove(entityToDelete);
                
                // return number of changes
                return await _coditoContext.SaveChangesAsync();
            }
            else
            {
                return 0;
            }
            
        }

        public async Task<SalesRequestResult> ApplyCustomizationSaleAsync(int id)
        {
            bool exists = await CustomizationExistsAsync(id);
           
            if (exists)
            {
                var customization = await _coditoContext.Customizations.FindAsync(id);
                if (customization.InventoryLevel > 0)
                {
                    customization.Sell();
                    _coditoContext.Customizations.Update(customization);
                    await _coditoContext.SaveChangesAsync();
                    return SalesRequestResult.Accepted;
                }
                else
                {
                    return SalesRequestResult.OutOfStock;
                }               
            }
            else
            {
                return SalesRequestResult.NotFound;
            }          
        }       
    }
}
