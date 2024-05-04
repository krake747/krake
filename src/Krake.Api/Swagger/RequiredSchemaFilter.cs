using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Krake.Api.Swagger;

internal sealed class RequiredSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var requiredProperties = context.Type
            .GetProperties()
            .Where(x => x.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .ToList();

        var requiredJsonProperties = schema.Properties
            .Where(kvp => requiredProperties.Any(p => string.Equals(p.Name, kvp.Key, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        schema.Required = requiredJsonProperties
            .Select(x => x.Key)
            .ToHashSet();

        foreach (var requiredJsonProperty in requiredJsonProperties)
        {
            requiredJsonProperty.Value.Nullable = false;
        }
    }
}