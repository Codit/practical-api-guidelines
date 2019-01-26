using Codit.LevelOne.Entities;
using Codit.LevelOne.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;

namespace Codit.LevelOne.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Configure database
        /// </summary>
        /// <param name="services">Collections of services in application</param>
        /// <param name="configuration">Configuration properties</param>
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
        /// <param name="services">Collections of services in application</param>
        public static void ConfigureInvalidStateHandling(this IServiceCollection services)
        {
            services?.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = context.HttpContext.Request.Path,
                        Title = "Validation error",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = Constants.Messages.ProblemDetailsDetail,
                        Instance = $"urn:codit.eu:client-error:{Guid.NewGuid()}"
                    };
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { ContentTypeNames.Application.JsonProblem, ContentTypeNames.Application.XmlProblem}
                    };
                };
            });
        }

        /// <summary>
        ///     Configure the MVC stack
        /// </summary>
        /// <param name="services">Collections of services in application</param>
        public static void ConfigureMvc(this IServiceCollection services)
        {
            services?.AddMvc(cfg =>
                {
                    cfg.RespectBrowserAcceptHeader = true;
                    cfg.ReturnHttpNotAcceptable = true; // Return 406 for not acceptable media types
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
        /// <param name="services">Collections of services in application</param>
        public static void ConfigureOpenApiGeneration(this IServiceCollection services)
        {
            var xmlDocumentationPath = GetXmlDocumentationPath(services);

            services?.AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.EnableAnnotations();
                swaggerGenOptions.DescribeAllEnumsAsStrings();
                swaggerGenOptions.OperationFilter<SwaggerConsumesProducesOperationFilter>();
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
                if (string.IsNullOrEmpty(xmlDocumentationPath) == false)
                {
                    swaggerGenOptions.IncludeXmlComments(xmlDocumentationPath);
                }
            });
        }

        /// <summary>
        ///     Configure routing
        /// </summary>
        public static void ConfigureRouting(this IServiceCollection services)
        {
            services.AddRouting(configureOptions => configureOptions.LowercaseUrls = true);
        }

        private static string GetXmlDocumentationPath(IServiceCollection services)
        {
            var hostingEnvironment = services.FirstOrDefault(service => service.ServiceType == typeof(IHostingEnvironment));
            if (hostingEnvironment == null)
            {
                return string.Empty;
            }

            var contentRootPath = ((IHostingEnvironment)hostingEnvironment.ImplementationInstance).ContentRootPath;
            var xmlDocumentationPath = $"{contentRootPath}/Open-Api-Docs.xml";

            return File.Exists(xmlDocumentationPath) ? xmlDocumentationPath : string.Empty;
        }
    }
}