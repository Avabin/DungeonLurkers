using Autofac.Extensions.DependencyInjection;
using PierogiesBot.Host;
using Serilog;

var builder = CreateDefaultHostBuilder(args);
builder.ConfigureWebHostDefaults(x => x.UseStartup<Startup>());
var app = builder.Build();
app.Run();

static IHostBuilder CreateDefaultHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog((context, c) =>
                        c.MinimumLevel.Verbose().WriteTo.Console().WriteTo.File("logs/pierogiesbot.log"))
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());