using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EcommerceApi.Api.Infrastructure.OpenApi;

/// <summary>
/// Makes Swashbuckle render enum types as strings (member names) instead of integers.
/// </summary>
public class StringEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        schema.Type = "string";
        schema.Format = null;
        schema.Enum = Enum.GetNames(context.Type)
            .Select(name => (IOpenApiAny)new OpenApiString(name))
            .ToList();
    }
}
