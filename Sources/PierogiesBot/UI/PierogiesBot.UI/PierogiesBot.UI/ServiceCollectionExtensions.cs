using Autofac;
using PierogiesBot.Shared.Features;
using PierogiesBot.UI.Views;
using Shared.UI.IoC;
using Shared.UI.ViewModels;
using Shared.UI.ViewModels.MainView;

namespace PierogiesBot.UI;

public static class ServiceCollectionExtensions
{
    public static ContainerBuilder AddPierogiesBotUiInfrastructure(this ContainerBuilder builder)
    {
        // var viewModelsAssembly = typeof(SessionsViewModel).Assembly;
        var viewsAssembly = typeof(MainView).Assembly;

        builder.AddSharedUiServices<IPierogiesBotApi>()
            .AddScreen<MainViewModel>()
            .AddViews(viewsAssembly)
            // .AddViewModels(viewModelsAssembly)
            .AddSharedViewModels();

        return builder;
    }
}