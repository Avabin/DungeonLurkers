using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using RestEase;
using Splat;
using TheDungeonGuide.Shared.Features;
using TheDungeonGuide.UI.Shared.Features.HostScreen;

namespace TheDungeonGuide.UI.Shared.Features.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IHostEnvironment environment, Lazy<IServiceProvider> serviceProvider)
    {
        services.AddOptions();
            
        services.AddSingleton(MessageBus.Current);
        services.AddSingleton(environment.ContentRootFileProvider);

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();

        Locator.CurrentMutable.RegisterLazySingleton(() => new AutofacViewLocator(serviceProvider), typeof(IViewLocator));
        return services;
    }

    public static ContainerBuilder AddUiServices(this ContainerBuilder builder)
    {
        builder.RegisterType<UserStore.AppUserStore>().AsImplementedInterfaces().SingleInstance();
        builder.Register(ctx =>
        {
            var config = ctx.Resolve<IConfiguration>();
            
            var apiUrl = config["ApiUrl"];

            return RestClient.For<ITheDungeonGuideApi>(apiUrl);
        }).AsImplementedInterfaces().SingleInstance();

        return builder;
    }
}