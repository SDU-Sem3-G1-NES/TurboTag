using API.Controllers;
using API.Services;
using API.Repositories;
using Autofac;

namespace API;

public class DependencyRegistrations : Module
{
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
    }
}