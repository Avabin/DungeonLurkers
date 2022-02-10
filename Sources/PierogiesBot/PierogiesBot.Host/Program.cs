using Autofac.Extensions.DependencyInjection;
using PierogiesBot.Host;
using Serilog;

var builder = CreateDefaultHostBuilder(args);
builder.ConfigureWebHostDefaults(x => x.UseStartup<Startup>());
var app = builder.Build();
app.Run();
const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{SourceContext}] [{Level:u4}] {Message:lj}{NewLine} {Exception}";
static IHostBuilder CreateDefaultHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, builder) => builder.AddEnvironmentVariables(prefix: "PierogiesBot_"))
        .UseSerilog((context, c) =>
         {
             var serverUrl = context.Configuration["Seq:Url"];
             var apiKey    = context.Configuration["Seq:ApiKey"];
             c.MinimumLevel.Verbose();
             if (!string.IsNullOrEmpty(serverUrl) && !string.IsNullOrEmpty(apiKey))
             {
                 c.WriteTo.Seq(serverUrl, apiKey: apiKey).Enrich.FromLogContext().Enrich.WithProperty("Service", "PierogiesBot");
             }
             c.WriteTo.Console(outputTemplate: outputTemplate).Enrich.FromLogContext()
              .WriteTo.File("logs/pierogiesbot.log", outputTemplate: outputTemplate).Enrich.FromLogContext();
         })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());