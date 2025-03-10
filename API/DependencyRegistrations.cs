using API.Controllers;
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
    }
}