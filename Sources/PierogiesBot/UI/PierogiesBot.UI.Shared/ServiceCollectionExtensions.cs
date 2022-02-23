using Autofac;
using PierogiesBot.Shared.Features;
using PierogiesBot.UI.Shared.Features.BotCrontabRules;

namespace PierogiesBot.UI.Shared;

public static class ServiceCollectionExtensions
{
    public static ContainerBuilder AddUiServices(this ContainerBuilder builder)
    {

        builder.RegisterType<CrontabRulesService>().AsImplementedInterfaces();

        return builder;
    }
}