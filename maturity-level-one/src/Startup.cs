using Codit.LevelOne.DB;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codit.LevelOne
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure database
            services.ConfigureDatabase(Configuration);

            // Configure API
            services.AddApiVersioning();
            services.ConfigureMvc();
            services.ConfigureOpenApiGeneration();
            services.ConfigureRouting();
            services.ConfigureInvalidStateHandling();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WorldCupContext worldCupContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Seed DB
            worldCupContext.DataSeed();

            // Configure API
            app.UseHttpsRedirection();
            app.UseExceptionHandlerWithProblemJson();
            app.UseMvc();
            app.UseOpenApi();

            // Configure Automapper
            app.UseAutoMapper(cfg =>
            {
                cfg.CreateMap<Entities.Team, Models.TeamDto>();
                cfg.CreateMap<Entities.Team, Models.TeamDetailsDto>();
                cfg.CreateMap<Entities.Player, Models.PlayerDto>();
                cfg.CreateMap<Models.PlayerDto, Entities.Player>();
            });
        }
    }
}
