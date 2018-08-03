using System;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Codit.LevelOne.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseAutoMapper(this IApplicationBuilder applicationbuilder, Action<IMapperConfigurationExpression> mapperConfigExpression)
        {
            Mapper.Initialize(mapperConfigExpression);
        }

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

                    var problemDetails = new ProblemDetails
                    {
                        Title = "An unexpected error occurred!",
                        Status = 500,
                        Detail = errorDetail,
                        Instance = $"urn:codit:error:{Guid.NewGuid()}"
                    };

                    // TODO: Plug in telemetry

                    context.Response.WriteJson(problemDetails, contentType: "application/problem+json");
                });
            });
        }

        public static void UseOpenApi(this IApplicationBuilder applicationbuilder)
        {
            applicationbuilder.UseSwagger();
            applicationbuilder.UseSwaggerUI(c => { c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: Constants.OpenApi.Title); });
        }
    }
}