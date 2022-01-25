using Autofac;
using PierogiesBot.Persistence.BotMessageSubscription.Features.Many;
using PierogiesBot.Persistence.BotMessageSubscription.Features.Single;

namespace PierogiesBot.Persistence.BotMessageSubscription.Features;

public static class ContainerBuilderExtensions
{
    public static void AddBotMessageSubscriptionsMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleBotMessageSubscriptionService>().AsImplementedInterfaces();
        builder.RegisterType<ManyBotMessageSubscriptionsService>().AsImplementedInterfaces();
        builder.RegisterType<BotMessageSubscriptionFacade>().AsImplementedInterfaces();
    }
}