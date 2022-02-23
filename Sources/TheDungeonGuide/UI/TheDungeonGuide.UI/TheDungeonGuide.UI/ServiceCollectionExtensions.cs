using Autofac;
using Shared.UI.IoC;
using Shared.UI.ViewModels;
using Shared.UI.ViewModels.MainView;
using TheDungeonGuide.Shared.Features;
using TheDungeonGuide.UI.ViewModels.Features.SessionsView;
using TheDungeonGuide.UI.Views;

namespace TheDungeonGuide.UI;

public static class ServiceCollectionExtensions
{
    public static ContainerBuilder AddTdgUiInfrastructure(this ContainerBuilder builder)
    {
        var viewModelsAssembly = typeof(SessionsViewModel).Assembly;
        var viewsAssembly = typeof(MainView).Assembly;
        builder
            .AddScreen<DefaultMainViewModel>()
            .AddViews(viewsAssembly)
            .AddViewModels(viewModelsAssembly)
            .AddSharedUiServices<ITheDungeonGuideApi>()
            .AddSharedViewModels();

        return builder;
    }
}