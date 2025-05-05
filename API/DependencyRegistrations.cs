using API.DataAccess;
using API.Repositories;
using API.Services;
using Autofac;
using DotNetEnv;

namespace API;

public class DependencyRegistrations : Module
{
    private readonly string _mongoHost = Env.GetString("MONGODB_HOST");
    private readonly string _mongoPass = Env.GetString("MONGO_INITDB_ROOT_PASSWORD");
    private readonly string _mongoPort = Env.GetString("MONGODB_PORT");
    private readonly string _mongoUser = Env.GetString("MONGO_INITDB_ROOT_USERNAME");
    private readonly string _postgresHost = Env.GetString("POSTGRES_HOST");
    private readonly string _postgresPassword = Env.GetString("POSTGRES_PASSWORD");
    private readonly string _postgresPort = Env.GetString("POSTGRES_PORT");
    private readonly string _postgresUser = Env.GetString("POSTGRES_USER");

    protected override void Load(ContainerBuilder builder)
    {
        // Automatically register all types that implement IServiceBase
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IServiceBase).IsAssignableFrom(t));
        // Automatically register all types that implement IRepositoryBase
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IRepositoryBase).IsAssignableFrom(t));
        // Register SqlDataAccess
        builder.RegisterType<SqlDataAccess>()
            .As<ISqlDbAccess>()
            .WithParameter("connectionString",
                $"Host={_postgresHost};Port={_postgresPort};User Id={_postgresUser};Password={_postgresPassword};");
        // Register the MongoDB client
        builder.RegisterType<MongoDataAccess>()
            .As<IMongoDataAccess>()
            .WithParameter("connectionString", $"mongodb://{_mongoUser}:{_mongoPass}@{_mongoHost}:{_mongoPort}");
    }
}