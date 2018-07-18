using Codit.LevelOne.DB;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;

namespace Codit.LevelOne
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }
      public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();



            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Startup.Configuration.GetConnectionString("WorldCupDB");
            //default scope lifetime
#if DEBUG
            services.AddDbContext<WorldCupContext>(opt => opt.UseInMemoryDatabase("WorldCupDB"));
#else
            services.AddDbContext<WorldCupContext>(o => o.UseSqlServer(connectionString)); 
#endif
            services.AddScoped<IWorldCupRepository, WorldCupRepository>(); //scoped

            // Versioning
            services.AddApiVersioning();

            services
                .AddMvc(cfg =>
                {
                    cfg.RespectBrowserAcceptHeader = true;
                    cfg.ReturnHttpNotAcceptable = true;
                    //cfg.InputFormatters.Add(new XmlSerializerInputFormatter(cfg));
                    //cfg.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .AddJsonOptions(opt =>
                {
                    //explicit datetime configuration
                    opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    opt.SerializerSettings.DateFormatString = "o";
                    opt.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        CamelCaseText = false
                    });
                });

            // Routing naming convention
            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddSwaggerGen(cfg =>
            {
                cfg.DescribeAllEnumsAsStrings();
                cfg.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = Constants.OpenApi.Title,
                    Description = Constants.OpenApi.Description,
                    TermsOfService = Constants.OpenApi.TermsOfService,
                    Contact = new Contact()
                    {
                        Name = Constants.OpenApi.ContactName,
                        Email = Constants.OpenApi.ContactEmail,
                        Url = Constants.OpenApi.ContactUrl
                    }
                });
            });

            //Problem+Json
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://asp.net/core",
                        Detail = Constants.Messages.ProblemDetailsDetail
                    };
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WorldCupContext worldCupContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            // seed DB
            worldCupContext.DataSeed();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Constants.OpenApi.Title);
            });

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<LevelOne.Entities.Team, LevelOne.Models.TeamDto>();
                cfg.CreateMap<LevelOne.Entities.Team, LevelOne.Models.TeamDetailsDto>();
                cfg.CreateMap<LevelOne.Entities.Player, LevelOne.Models.PlayerDto>();
                cfg.CreateMap<LevelOne.Models.PlayerDto, LevelOne.Entities.Player>();
            });

        }
    }
}
