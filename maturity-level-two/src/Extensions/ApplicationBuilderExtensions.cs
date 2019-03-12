using System;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Codit.LevelOne.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Configure to use AutoMapper
        /// </summary>
        /// <param name="applicationbuilder">Application builder to use</param>
        /// <param name="mapperConfigExpression">AutoMapper configuration to use</param>
        public static void UseAutoMapper(this IApplicationBuilder applicationbuilder, Action<IMapperConfigurationExpression> mapperConfigExpression)
        {
            Mapper.Initialize(mapperConfigExpression);
        }

        /// <summary>
        ///     Configure to use global exception handler with application/problem+json
        /// </summary>
        /// <param name="applicationBuilder">Application builder to use</param>
        public static void UseExceptionHandlerWithProblemJson(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder?.UseExceptionHandler(errorApplication =>
            {
                errorApplication.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    var errorDetail = context.Request.IsLocalRequest()
                        ? exception.Demystify().ToString()
                        : "The instance value should be used to identify the problem when calling customer support";

                    var problemDetails = new ProblemDetailsError
                    {
                        Title = "An unexpected error occurred!",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = errorDetail,
                        Instance = $"urn:codit.eu:server-error:{Guid.NewGuid()}"
                    };

                    // TODO: Plug in telemetry
                    context.Response.WriteJson(problemDetails, contentType: ContentTypeNames.Application.JsonProblem);
                });
            });
        }

        /// <summary>
        ///     Configure to use OpenAPI with UI
        /// </summary>
        /// <param name="applicationbuilder">Application builder to use</param>
        public static void UseOpenApi(this IApplicationBuilder applicationbuilder)
        {
            applicationbuilder.UseSwagger(UseLowercaseUrls);
            applicationbuilder.UseSwaggerUI(opt => 
            {
                opt.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: Constants.OpenApi.Title);
                opt.DisplayOperationId();
                opt.EnableDeepLinking();
                opt.DocumentTitle = Constants.OpenApi.Title;
                opt.DocExpansion(DocExpansion.List);
                opt.DisplayRequestDuration();
                opt.EnableFilter();
            });
        }

        // Makes sure that the urls are lower case. See https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/74
        private static void UseLowercaseUrls(SwaggerOptions swaggerOptions)
        {
            swaggerOptions.PreSerializeFilters.Add((document, request) => { document.Paths = document.Paths.ToDictionary(p => p.Key.ToLowerInvariant(), p => p.Value); });
        }
    }
}