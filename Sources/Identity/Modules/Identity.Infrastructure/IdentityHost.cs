using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Identity.Infrastructure;

public static class IdentityHost
{
    public static IHostBuilder CreateDefaultHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .UseSerilog((context, c) =>
                                   c.MinimumLevel.Verbose().WriteTo.Console().WriteTo.File("logs/identity.log"))
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}