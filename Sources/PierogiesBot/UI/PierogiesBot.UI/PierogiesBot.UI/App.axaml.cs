using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PierogiesBot.UI.Views;
using ReactiveUI;
using Shared.UI.IoC;
using Shared.UI.Login;
using Shared.UI.ViewModels.LoginView;
using Shared.UI.ViewModels.MainView;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

namespace PierogiesBot.UI;

public partial class App : Application
{
    public delegate void                               ConfigurePlatformServicesDelegate(IServiceCollection services);
    public static   ConfigurePlatformServicesDelegate? ConfigurePlatformServices { get; set; }
    private         IHost                              _host;

    public App()
    {
        Init();
    }
    
    public IServiceProvider Container { get; private set; }

    private void Init()
    {
        var host = Host
                  .CreateDefaultBuilder()
                  .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                  .ConfigureServices(services =>
                   {
                       services.AddInfrastructure();

                       // Configure our local services and access the host configuration
                       ConfigurePlatformServices?.Invoke(services);
                   })
                  .ConfigureContainer<ContainerBuilder>(builder =>
                   {
                       Locator.CurrentMutable.RegisterLazySingleton(() => new AutofacViewLocator(), typeof(IViewLocator));
                       builder.AddPierogiesBotUiInfrastructure();
                   })
                  .ConfigureLogging(loggingBuilder =>
                   {
                       loggingBuilder.AddSplat();
                   })
                  .Build();

        // Since MS DI container is a different type,
        // we need to re-register the built container with Splat again
        _host     = host;
        Container = _host.Services;
        Container.UseMicrosoftDependencyResolver();
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += (sender, args) =>
            {
                _host.StopAsync(TimeSpan.FromSeconds(3));
                _host.WaitForShutdown();
            };
            desktop.Exit += (sender, args) =>
            {
                _host.StopAsync(TimeSpan.FromSeconds(3));
                _host.WaitForShutdown();
            };
            _host.Start();
            desktop.MainWindow = new MainWindow
            {
                DataContext = Locator.Current.GetService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Locator.Current.GetService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}