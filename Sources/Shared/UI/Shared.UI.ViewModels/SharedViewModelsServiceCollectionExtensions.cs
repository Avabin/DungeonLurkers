using Autofac;
using ReactiveUI;
using Shared.UI.ViewModels.LoginView;

namespace Shared.UI.ViewModels;

public static class SharedViewModelsServiceCollectionExtensions
{
    public static ContainerBuilder AddSharedViewModels(this ContainerBuilder builder)
    {
        // Load view models but not screen
        var assembly = typeof(LoginViewModel).Assembly;
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("ViewModel") && !t.IsInterface && t.GetInterfaces().All(x => x != typeof(IScreen)))
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        return builder;
    }
}