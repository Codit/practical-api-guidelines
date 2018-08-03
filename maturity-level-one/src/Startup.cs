using Codit.LevelOne.DB;
using Codit.LevelOne.Entities;
using Codit.LevelOne.Extensions;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("WorldCupDB");
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
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
            else
            {
                app.UseHsts();
            }
            
            // Seed DB
            worldCupContext.DataSeed();

            // Configure API
            app.UseMvc();
            app.UseHttpsRedirection();
            app.UseExceptionHandlerWithProblemJson();
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
