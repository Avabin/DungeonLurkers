using Autofac.Extensions.DependencyInjection;
using Serilog;

namespace TheDungeonGuide.Sessions.Host;

public static class Program
{
    const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{SourceContext}] [{Level:u4}] {Message:lj}{NewLine} {Exception}";
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration(b => b.AddEnvironmentVariables(prefix: "TheDungeonGuide_"))
                        .UseSerilog((context, c) =>
                         {
                             var serverUrl = context.Configuration["Seq:Url"];
                             var apiKey    = context.Configuration["Seq:ApiKey"];
                             c.MinimumLevel.Verbose();
                             if (!string.IsNullOrEmpty(serverUrl) && !string.IsNullOrEmpty(apiKey))
                             {
                                 c.WriteTo.Seq(serverUrl, apiKey: apiKey).Enrich.FromLogContext().Enrich.WithProperty("Service", "TDG-Sessions");
                             }
                             c.WriteTo.Console(outputTemplate: OutputTemplate).Enrich.FromLogContext()
                              .WriteTo.Console().WriteTo.File("logs/sessions.log", outputTemplate: OutputTemplate);
                         })
                        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}