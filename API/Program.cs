using System.Text;
using API;
using API.DataAccess;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapper;
using DotNetEnv;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NSwag;

var builder = WebApplication.CreateBuilder(args);
Env.Load("../.env");

SqlMapper.AddTypeHandler(new GenericListHandler<int>());

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
builder.Services.AddHttpClient("OllamaClient", client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerOperationIdFilter>();

    // Add JWT authentication support
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your valid JWT token (\"accessToken\") from the login endpoint.\n\nExample: eyJhbGciOiJIUzI1..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

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

                if (schema?.Reference?.Id != null && schema.Reference.Id.StartsWith("PagedResult"))
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

// Add Hangfire services
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection($"Host={Env.GetString("POSTGRES_HOST")};Port={Env.GetString("POSTGRES_PORT")};Database={Env.GetString("HANGFIRE_DB")};Username={Env.GetString("POSTGRES_USER")};Password={Env.GetString("POSTGRES_PASSWORD")}");
        });
});

// Add Hangfire server
builder.Services.AddHangfireServer();

// Configure authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Env.GetString("JWT_SECRET_KEY"))),
        };
    });

// Add authorization
builder.Services.AddAuthorization();

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
    app.UseHangfireDashboard();
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();