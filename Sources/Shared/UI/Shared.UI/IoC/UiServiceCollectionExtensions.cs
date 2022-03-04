using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using RestEase;
using Shared.Features.Authentication;
using Shared.UI.Authentication;
using Shared.UI.HostScreen;
using Shared.UI.Login;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.Users;
using Shared.UI.UserStore;
using Splat;

namespace Shared.UI.IoC;

public static class UiServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions();
            
        services.AddSingleton(MessageBus.Current);
        services.AddSingleton<IFileProvider>((sp) => sp.GetRequiredService<IHostEnvironment>().ContentRootFileProvider);

        services.AddHostedService<AuthenticationHostedService>();

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        return services;
    }
    
    public static ContainerBuilder AddViews(this ContainerBuilder builder, Assembly assembly)
    {
        // Load views
        builder.RegisterAssemblyTypes(assembly)
               .Where(t => t.GetInterfaces()
                            .Any(
                                 i => i.IsGenericType
                                   && i.GetGenericTypeDefinition() == typeof(IViewFor<>)))
               .AsSelf()
               .AsImplementedInterfaces();
        
        return builder;
    }

    // AddViewModels
    public static ContainerBuilder AddViewModels(this ContainerBuilder builder, Assembly assembly)
    {
        // Load view models but not screen
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("ViewModel") && !t.IsInterface && t.GetInterfaces().All(x => x != typeof(IScreen)))
            .AsSelf()
            .AsImplementedInterfaces();

        return builder;
    }

    public static ContainerBuilder AddScreen<T>(this ContainerBuilder builder) where T : IHostScreenViewModel
    {
        builder.RegisterType<T>()
            .AsSelf()
            .As<IScreen>()
            .As<IHostScreenViewModel>()
            .AsImplementedInterfaces()
            .SingleInstance();
        return builder;
    }

    public static ContainerBuilder AddSharedUiServices<T>(this ContainerBuilder builder) where T : IAuthenticatedApi
    {
        builder.RegisterType<AppUserService>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<LoginService>().AsImplementedInterfaces();
        builder.RegisterType<UsersService>().AsImplementedInterfaces();
        builder.RegisterType<RoutableViewModelFactory>().AsImplementedInterfaces();
        builder.Register(ctx =>
        {
            var config = ctx.Resolve<IConfiguration>();
            
            var apiUrl = config["ApiUrl"];
            var apiPathPrefix = config["ApiPathPrefix"] ?? "";
            var identityPathPrefix = config["IdentityPathPrefix"];

            var client = RestClient.For<T>(apiUrl);

            client.IdentityPathPrefix = identityPathPrefix;
            client.PathPrefix         = apiPathPrefix;
            
            return client;
        }).AsImplementedInterfaces().SingleInstance();

        return builder;
    }
}