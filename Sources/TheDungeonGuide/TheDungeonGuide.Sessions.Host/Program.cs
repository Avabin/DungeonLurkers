using Autofac.Extensions.DependencyInjection;
using Serilog;

namespace TheDungeonGuide.Sessions.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration(b => b.AddEnvironmentVariables(prefix: "TheDungeonGuide_"))
                        .UseSerilog((context, c) =>
                                        c.MinimumLevel.Verbose().WriteTo.Console().WriteTo.File("logs/sessions.log"))
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}