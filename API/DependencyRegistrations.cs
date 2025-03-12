using API.Controllers;
using API.Services;
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
        // Automatically register all types that implement IAdminService
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IAdminService).IsAssignableFrom(t));
        // Automatically register all types that implement ILibraryService
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(ILibraryService).IsAssignableFrom(t));
        // Automatically register all types that implement IUploadService
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IUploadService).IsAssignableFrom(t));
        // Automatically register all types that implement IUserCredentialsService
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .AsImplementedInterfaces()
            .Where(t => typeof(IUserCredentialsService).IsAssignableFrom(t));
    }
}