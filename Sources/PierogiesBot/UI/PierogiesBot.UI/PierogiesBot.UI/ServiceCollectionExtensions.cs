using Autofac;
using JetBrains.Annotations;
using PierogiesBot.Shared.Features;
using PierogiesBot.UI.Shared;
using PierogiesBot.UI.ViewModels.Features.BotCrontabRules;
using PierogiesBot.UI.ViewModels.Features.MainView;
using PierogiesBot.UI.Views;
using Shared.UI.IoC;
using Shared.UI.ViewModels;
using Shared.UI.ViewModels.MainView;

namespace PierogiesBot.UI;

public static class ServiceCollectionExtensions
{
    public static ContainerBuilder AddPierogiesBotUiInfrastructure(this ContainerBuilder builder)
    {
        var viewModelsAssembly = typeof(CrontabRulesViewModel).Assembly;
        var viewsAssembly      = typeof(MainView).Assembly;

        builder.AddSharedUiServices<IPierogiesBotApi>()
               .AddUiServices()
               .AddViewModels(viewModelsAssembly)
               .AddScreen<PierogiesBotMainViewModel>()
               .AddViews(viewsAssembly)
               .AddSharedViewModels();

        return builder;
    }
}