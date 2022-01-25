using Autofac;
using PierogiesBot.Persistence.BotResponseRules.Features.Many;
using PierogiesBot.Persistence.BotResponseRules.Features.Single;

namespace PierogiesBot.Persistence.BotResponseRules.Features;

public static class ContainerBuilderExtensions
{
    public static void AddBotResponseRulesMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleBotResponseRuleService>().AsImplementedInterfaces();
        builder.RegisterType<ManyBotResponseRulesService>().AsImplementedInterfaces();
        builder.RegisterType<BotResponseRuleFacade>().AsImplementedInterfaces();
    }
}