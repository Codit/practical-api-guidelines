using Codit.LevelOne.Entities;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;

namespace Codit.LevelOne.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Configure database
        /// </summary>
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                return;
            }

            var connectionString = configuration.GetConnectionString(name: "WorldCupDB");
            //default scope lifetime
#if DEBUG
            services.AddDbContext<WorldCupContext>(opt => opt.UseInMemoryDatabase(databaseName: "WorldCupDB"));
#else
            services.AddDbContext<WorldCupContext>(o => o.UseSqlServer(connectionString)); 
#endif
            services.AddScoped<IWorldCupRepository, WorldCupRepository>(); //scoped
        }

        /// <summary>
        ///     Configure how to handle invalid state with problem+json
        /// </summary>
        public static void ConfigureInvalidStateHandling(this IServiceCollection services)
        {
            services?.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://codit.eu/validation",
                        Detail = Constants.Messages.ProblemDetailsDetail
                    };
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = {"application/problem+json", "application/problem+xml"}
                    };
                };
            });
        }

        /// <summary>
        ///     Configure the MVC stack
        /// </summary>
        public static void ConfigureMvc(this IServiceCollection services)
        {
            services?.AddMvc(cfg =>
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
        }

        /// <summary>
        ///     Configure OpenAPI generation
        /// </summary>
        public static void ConfigureOpenApiGeneration(this IServiceCollection services)
        {
            services?.AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.DescribeAllEnumsAsStrings();
                swaggerGenOptions.SwaggerDoc(name: "v1", info: new Info
                {
                    Version = "v1",
                    Title = Constants.OpenApi.Title,
                    Description = Constants.OpenApi.Description,
                    TermsOfService = Constants.OpenApi.TermsOfService,
                    Contact = new Contact
                    {
                        Name = Constants.OpenApi.ContactName,
                        Email = Constants.OpenApi.ContactEmail,
                        Url = Constants.OpenApi.ContactUrl
                    }
                });
            });
        }

        /// <summary>
        ///     Configure routing
        /// </summary>
        public static void ConfigureRouting(this IServiceCollection services)
        {
            services.AddRouting(configureOptions => configureOptions.LowercaseUrls = true);
        }
    }
}