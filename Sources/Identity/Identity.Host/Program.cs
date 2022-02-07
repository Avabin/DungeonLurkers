using Identity.Infrastructure;

namespace Identity.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        IdentityHost
           .CreateDefaultHostBuilder(args)
           .ConfigureAppConfiguration((context, builder) => builder.AddEnvironmentVariables(prefix:"Identity_"))
           .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}