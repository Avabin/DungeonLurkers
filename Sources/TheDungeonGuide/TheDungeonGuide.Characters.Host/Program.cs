using Autofac.Extensions.DependencyInjection;
using Serilog;

namespace TheDungeonGuide.Characters.Host;

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
                                        c.MinimumLevel.Verbose().WriteTo.Console().WriteTo.File("logs/characters.log"))
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}