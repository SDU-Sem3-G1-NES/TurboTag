using API.Controllers;
using API.DataAccess;
using API.Services;
using API.Repositories;
using Autofac;
using DotNetEnv;

namespace API;

public class DependencyRegistrations : Module
{
    private readonly string _mongoHost = Env.GetString("MONGODB_HOST");
    private readonly string _mongoPort = Env.GetString("MONGODB_PORT");
    private readonly string _mongoUser = Env.GetString("MONGO_INITDB_ROOT_USERNAME");
    private readonly string _mongoPass = Env.GetString("MONGO_INITDB_ROOT_PASSWORD");
    protected override void Load(ContainerBuilder builder)
    {
        // Automatically register all types that implement ITest
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(ITestBase).IsAssignableFrom(t));
        // Automatically register all types that implement IServiceBase
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IServiceBase).IsAssignableFrom(t));
        // Automatically register all types that implement IRepositoryBase
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IRepositoryBase).IsAssignableFrom(t));
        
        // Register the MongoDB client
        builder.RegisterType<MongoDataAccess>()
            .As<IMongoDataAccess>()
            .WithParameter("connectionString", $"mongodb://{_mongoUser}:{_mongoPass}@{_mongoHost}:{_mongoPort}");
    }
}