using Autofac;
using PierogiesBot.Persistence.BotCrontabRule.Features.Many;
using PierogiesBot.Persistence.BotCrontabRule.Features.Single;

namespace PierogiesBot.Persistence.BotCrontabRule.Features;

public static class ContainerBuilderExtensions
{
    public static void AddBotCrontabRulesMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleBotCrontabRuleService>().AsImplementedInterfaces();
        builder.RegisterType<ManyBotCrontabRulesService>().AsImplementedInterfaces();
        builder.RegisterType<BotCrontabRuleFacade>().AsImplementedInterfaces();
    }
}