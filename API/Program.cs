using API;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DotNetEnv;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NSwag;

var builder = WebApplication.CreateBuilder(args);
Env.Load(".env");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.OperationFilter<SwaggerOperationIdFilter>(); });
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });
;
builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        var pagedResultSchemas = new Dictionary<string, OpenApiSchema>();

        // Identify all response types that are PagedResult<T>
        foreach (var path in document.Paths)
        foreach (var operation in path.Value.Values)
            if (operation.Responses.ContainsKey("200") &&
                operation.Responses["200"].Content.ContainsKey("application/json"))
            {
                var schema = operation.Responses["200"].Content["application/json"].Schema;

                if (schema?.Reference != null && schema.Reference.Id.StartsWith("PagedResult"))
                {
                    var dtoTypeName = schema.Reference.Id.Replace("PagedResult", "");

                    if (!document.Components.Schemas.ContainsKey($"PagedResult{dtoTypeName}"))
                        document.Components.Schemas[$"PagedResult{dtoTypeName}"] = new JsonSchema
                        {
                            Type = JsonObjectType.Object,
                            Properties =
                            {
                                ["items"] = new JsonSchemaProperty
                                {
                                    Type = JsonObjectType.Array,
                                    Item = new JsonSchema
                                    {
                                        Reference = document.Components.Schemas.ContainsKey(dtoTypeName)
                                            ? document.Components.Schemas[dtoTypeName]
                                            : new JsonSchema { Type = JsonObjectType.Object }
                                    }
                                },
                                ["totalCount"] = new JsonSchemaProperty { Type = JsonObjectType.Integer },
                                ["pageSize"] = new JsonSchemaProperty { Type = JsonObjectType.Integer },
                                ["currentPage"] = new JsonSchemaProperty { Type = JsonObjectType.Integer },
                                ["totalPages"] = new JsonSchemaProperty { Type = JsonObjectType.Integer }
                            }
                        };

                    // Override response type to correctly reference PagedResult<T>
                    operation.Responses["200"].Content["application/json"].Schema =
                        document.Components.Schemas[$"PagedResult{dtoTypeName}"];
                }
            }
    };
    config.DocumentProcessors.Add(new PagedResultProcessor());
});


// Configure Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    containerBuilder.RegisterModule(new DependencyRegistrations()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 API");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

app.Run();