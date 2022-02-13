using System.Linq;
using Autofac;
using ReactiveUI;
using TheDungeonGuide.UI.Wpf.Features.MainView;

namespace TheDungeonGuide.UI.Wpf;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddViews(this ContainerBuilder builder)
    {
        var assembly = typeof(MainView).Assembly;
        
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
}