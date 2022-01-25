using Autofac;
using PierogiesBot.Persistence.BotReactRules.Features.Many;
using PierogiesBot.Persistence.BotReactRules.Features.Single;

namespace PierogiesBot.Persistence.BotReactRules.Features;

public static class ContainerBuilderExtensions
{
    public static void AddBotReactRulesMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleBotReactRuleService>().AsImplementedInterfaces();
        builder.RegisterType<ManyBotReactRulesService>().AsImplementedInterfaces();
        builder.RegisterType<BotReactRuleFacade>().AsImplementedInterfaces();
    }
}