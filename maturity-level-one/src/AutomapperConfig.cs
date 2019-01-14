using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codit.LevelOne
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
                        mapperConfig.CreateMap<Entities.Team, Models.TeamDto>();
                        mapperConfig.CreateMap<Entities.Team, Models.TeamDetailsDto>();
                        mapperConfig.CreateMap<Entities.Player, Models.PlayerDto>();
                        mapperConfig.CreateMap<Models.PlayerDto, Entities.Player>();
                    });
                    _initialized = true;
                }
            }
        }
    }
}
