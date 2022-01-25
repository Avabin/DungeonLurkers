using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Infrastructure;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    private readonly string _scope;

    public AuthorizeCheckOperationFilter(string scope) => _scope = scope;
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
         || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Unauthorized",
            });
            operation.Responses.Add("403", new OpenApiResponse
            {
                Description = "Forbidden",
            });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [
                         new OpenApiSecurityScheme
                         {
                             Description = "Add auth token",
                             Name        = "Authorization",
                             Type        = SecuritySchemeType.Http,
                             In          = ParameterLocation.Header,
                             Scheme      = "Bearer",
                             Reference = new OpenApiReference
                             {
                                 Type = ReferenceType.SecurityScheme,
                                 Id   = "Bearer",
                             },
                         }
                        ] = new[]
                        {
                            _scope,
                        },
                },
            };

        }
    }
}