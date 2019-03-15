using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Codit.LevelTwo.Entities;
using Codit.LevelTwo.Extensions;
using Microsoft.EntityFrameworkCore;



namespace Codit.LevelTwo.Services
{
    public class CoditoRepository : ICoditoRepository
    {

        public CoditoContext _coditoContext;

        public CoditoRepository(CoditoContext coditoContext)
        {
            _coditoContext = coditoContext;
        }

        public async Task<bool> CarExistsAsync(int id)
        {
            return await _coditoContext.Cars.AnyAsync(car => car.Id == id);
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

        public async Task ApplyPatchAsync<TEntity, TDto>(TEntity entityToUpdate, TDto dto) where TEntity : class
        {
            if (dto == null)
                throw new ArgumentNullException($"{nameof(dto)}", $"{nameof(dto)} cannot be null.");

            var properties = dto.GetFilledProperties();
            _coditoContext.Attach(entityToUpdate);

            foreach (var property in properties)
            {
                _coditoContext.Entry(entityToUpdate).Property(property).IsModified = true;
            }

            await _coditoContext.SaveChangesAsync();
        }

        public async Task DeleteCustomizationAsync(int id)
        {
            var entityToDelete = _coditoContext.Customizations.Find(id);
            _coditoContext.Customizations.Remove(entityToDelete);
            await _coditoContext.SaveChangesAsync();
        }

        public async Task ApplyCustomizationSaleAsync(Customization customization)
        {
            //var entity = await _coditoContext.Customizations.Where(cust => cust.Id == id).FirstOrDefaultAsync();
            customization.InventoryLevel --;
            customization.NumberSold++;

            _coditoContext.Customizations.Update(customization);

            await _coditoContext.SaveChangesAsync();
        }

        
    }
}
