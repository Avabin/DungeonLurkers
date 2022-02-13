using Autofac;
using ReactiveUI;
using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.ViewModels.Features.MainView;

namespace TheDungeonGuide.UI.ViewModels;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddViewModels(this ContainerBuilder builder)
    {
        var assembly = typeof(MainViewModel).Assembly;
        // Load view models but not screen
        builder.RegisterAssemblyTypes(assembly)
               .Where(t => t.Name.EndsWith("ViewModel") && !t.IsInterface && t.GetInterfaces().All(x => x != typeof(IScreen)))
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();

        // Load screen
        builder.RegisterAssemblyTypes(assembly)
               .Where(t => !t.IsInterface && !t.IsAbstract && t.GetInterfaces().Any(x => x == typeof(IScreen)))
               .AsSelf()
               .As<IScreen>()
               .As<IHostScreenViewModel>()
               .SingleInstance();

        return builder;
    }
}