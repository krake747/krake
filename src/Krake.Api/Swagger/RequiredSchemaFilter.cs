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

        var requiredJsonProps = schema.Properties
            .Where(kvp => requiredProperties.Any(p => string.Equals(p.Name, kvp.Key, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        schema.Required = requiredJsonProps
            .Select(x => x.Key)
            .ToHashSet();

        // Optionally set them non-nullable too.
        foreach (var requiredJsonProp in requiredJsonProps)
        {
            requiredJsonProp.Value.Nullable = false;
        }
    }
}