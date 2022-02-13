using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using TheDungeonGuide.UI.Shared.Features.HostBuilder;
using TheDungeonGuide.UI.Shared.Features.IoC;
using TheDungeonGuide.UI.ViewModels;

#pragma warning disable 8618

namespace TheDungeonGuide.UI.Wpf;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class HostedWpfApplication<TStartup> where TStartup : class
{
    private IHost                                    _host;
    private ILogger<HostedWpfApplication<TStartup>>? _logger;

    public HostedWpfApplication()
    {
    }

    // ReSharper disable once StaticMemberInGenericType
    public static IServiceProvider Container { get; private set; }

    private static IHostBuilder DefaultHostBuilder =>
        Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>
             ((_, builder) => builder.AddViewModels().AddViews())
            .UseSerilog((hostingContext, serilog) =>
                            serilog.MinimumLevel.Verbose().WriteTo.Console().Enrich.FromLogContext().WriteTo.File("logs/tdg-wpf.log").Enrich.FromLogContext())
            .UseStartup<TStartup>();

    public async Task InitializeAsync()
    {
        _host = DefaultHostBuilder.Build();

        Container = _host.Services;

        await _host.StartAsync();

        using var scope = _host.Services.CreateScope();

        _logger = scope.ServiceProvider.GetRequiredService<ILogger<HostedWpfApplication<TStartup>>>();
        _logger.LogInformation("Starting DeviceManager");
    }

    public T GetView<T>() where T : IViewFor => _host.Services.GetRequiredService<T>();

    public void Stop()
    {
        _host.StopAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false).GetAwaiter().GetResult();

        _host.Dispose();
    }
}