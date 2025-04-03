using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API;

[UsedImplicitly]
public class SwaggerOperationIdFilter : IOperationFilter
{
    private readonly Dictionary<string, string> _swaggerOperationIds = new();

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)) return;

        if (_swaggerOperationIds.ContainsKey(controllerActionDescriptor.Id))
        {
            operation.OperationId = _swaggerOperationIds[controllerActionDescriptor.Id];
        }
        else
        {
            var operationIdBaseName =
                $"{controllerActionDescriptor.ControllerName}_{controllerActionDescriptor.ActionName}";
            var operationId = operationIdBaseName;
            var suffix = 2;
            while (_swaggerOperationIds.Values.Contains(operationId)) operationId = $"{operationIdBaseName}{suffix++}";

            _swaggerOperationIds[controllerActionDescriptor.Id] = operationId;
            operation.OperationId = operationId;
        }

        // Extract the actual return type
        var responseType = context.MethodInfo.ReturnType;

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ActionResult<>))
            responseType = responseType.GetGenericArguments()[0];

        // Detect IEnumerable<T> and treat it as PagedResult<T>
        Type? itemType = null;
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            itemType = responseType.GetGenericArguments()[0];
        else if (responseType.IsArray) itemType = responseType.GetElementType();

        if (itemType != null)
        {
            var itemTypeName = itemType.Name;
            var concretePagedResultSchemaId = $"PagedResult_{itemTypeName}";

            // Define the concrete schema PagedResult_DtoName dynamically
            if (!context.SchemaRepository.Schemas.ContainsKey(concretePagedResultSchemaId))
                context.SchemaRepository.Schemas[concretePagedResultSchemaId] = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["items"] = new()
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = itemTypeName }
                            }
                        },
                        ["totalCount"] = new() { Type = "integer", Format = "int32" },
                        ["pageSize"] = new() { Type = "integer", Format = "int32" },
                        ["currentPage"] = new() { Type = "integer", Format = "int32" },
                        ["totalPages"] = new() { Type = "integer", Format = "int32" }
                    }
                };

            // Modify the OpenAPI response to reference PagedResult_DtoName
            foreach (var response in operation.Responses.Values)
            {
                response.Content.Remove("text/plain");
                response.Content.Remove("text/json");
                response.Content.Remove("application/json");

                response.Content["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = concretePagedResultSchemaId
                        }
                    }
                };
            }
        }
    }
}