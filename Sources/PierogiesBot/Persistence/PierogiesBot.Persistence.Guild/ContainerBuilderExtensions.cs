using Autofac;
using PierogiesBot.Persistence.Guild.Features;
using PierogiesBot.Persistence.Guild.Features.Many;
using PierogiesBot.Persistence.Guild.Features.Single;

namespace PierogiesBot.Persistence.Guild;

public static class ContainerBuilderExtensions
{
    public static void AddGuildsMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<GuildFacade>().AsImplementedInterfaces();
        builder.RegisterType<ManyGuildsService>().AsImplementedInterfaces();
        builder.RegisterType<SingleGuildService>().AsImplementedInterfaces();
    }
}