using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Codit.LevelOne.Extensions
{
    public class SwaggerConsumesProducesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.TryGetMethodInfo(out var methodInfo))
            {
                return;
            }
            var requestAttributes = methodInfo.GetCustomAttributes(true).OfType<SwaggerConsumesProducesAttribute>().FirstOrDefault();

            if (requestAttributes != null)
            {
                if (requestAttributes.Clear)
                {
                    operation.Consumes.Clear();
                    operation.Produces.Clear();
                }

                var consumedTypes = from contentType in requestAttributes.Consumes.Split(',') select contentType.Trim();
                foreach (string ct in consumedTypes)
                {
                    if (!string.IsNullOrEmpty(ct)) operation.Consumes.Add(ct);
                }
                var producedTypes = from contentType in requestAttributes.Produces.Split(',') select contentType.Trim();
                foreach (string ct in producedTypes)
                {
                    if (!string.IsNullOrEmpty(ct)) operation.Produces.Add(ct);
                }
            }
        }
    }

}