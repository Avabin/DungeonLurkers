using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Hosting;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using ReactiveUI;
using Serilog;
using Shared.UI.HostBuilder;
using Shared.UI.IoC;
using Splat;

namespace PierogiesBot.UI.NetCore;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => AppMain(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .LogToTrace()
                     .UseReactiveUI();

    public static IHostBuilder BuildAvaloniaHost(string[] args)
        => Host.CreateDefaultBuilder(args)
               .UseServiceProviderFactory(new AutofacServiceProviderFactory())
               .UseSerilog((hostingContext, serilog) =>
                               serilog.MinimumLevel.Verbose().WriteTo.Console().Enrich.FromLogContext().WriteTo.File("logs/tdg-desktop.log").Enrich.FromLogContext())
               .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    Locator.CurrentMutable.RegisterLazySingleton(() => new AutofacViewLocator(), typeof(IViewLocator));
                    builder.AddPierogiesBotUiInfrastructure();

                }).UseStartup<Startup>();

    // Application entry point. Avalonia is completely initialized.
    private static void AppMain(string[] args)
    {
        IconProvider.Register<MaterialDesignIconProvider>();
        var app = BuildAvaloniaApp();

        var host = BuildAvaloniaHost(args).Build();
        ServiceLocator.Instance = host.Services;

        host.Start();

        app.StartWithClassicDesktopLifetime(args, ShutdownMode.OnLastWindowClose);

        host.StopAsync(TimeSpan.FromSeconds(5));

        host.WaitForShutdown();
    }
}