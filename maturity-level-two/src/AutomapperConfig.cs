using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Codit.LevelTwo
{
    public static class AutoMapperConfig
    {
        private static object _thisLock = new object();
        private static bool _initialized = false;
        
        public static void Initialize()
        {
            lock (_thisLock)
            {
                if (!_initialized)
                {
                    AutoMapper.Mapper.Initialize(mapperConfig =>
                    {
                        mapperConfig.CreateMap<Entities.Car, Models.CarDto>();
                        mapperConfig.CreateMap<Entities.Customization, Models.CustomizationDto>();
                        mapperConfig.CreateMap<Models.CustomizationDto, Entities.Customization>();
                    });
                    _initialized = true;
                }
            }
        }
    }
}
