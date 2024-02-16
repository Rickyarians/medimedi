using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CRUDServices.Extensions
{
    public class AddRequiredHeaderMenuId : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            // bool isAuthorized = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
            //context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            var allowAnonymous =
                (context.MethodInfo?.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ?? true) ||
                (context.MethodInfo?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ?? true);

            //if (isAuthorized && !allowAnonymous)
            if (!allowAnonymous)
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "MenuId",
                    In = ParameterLocation.Header,
                    Description = "Header Parameter MenuId",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}