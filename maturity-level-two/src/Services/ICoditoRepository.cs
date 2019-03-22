using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Codit.LevelTwo.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Codit.LevelTwo.Services
{
    public interface ICoditoRepository
    {
        Task<bool> CarExistsAsync(int id);

        Task<bool> CustomizationExistsAsync(int id);

        Task<IEnumerable<Car>> GetCarsAsync(CarBodyType? bodyType);

        Task<Car> GetCarAsync(int id, bool includeCustomization);

        Task<IEnumerable<Customization>> GetAllCustomizationsAsync();

        Task<Customization> GetCustomizationAsync(int id);

        Task CreateCustomizationAsync(Customization customization);
        //Task ApplyPatchAsync<TEntity, TDto>(TEntity entityToUpdate, TDto dto) where TEntity : class;
        Task ApplyPatchAsync<TEntity>(TEntity entityUpdated) where TEntity : class;


        Task ApplyPatchAsync<TEntity, TDto>(TEntity entityToUpdate, TDto dto) where TEntity : class;

        Task<SalesRequestResult> ApplyCustomizationSaleAsync(int id);

        Task<int> DeleteCustomizationAsync(int id);
    }
}
