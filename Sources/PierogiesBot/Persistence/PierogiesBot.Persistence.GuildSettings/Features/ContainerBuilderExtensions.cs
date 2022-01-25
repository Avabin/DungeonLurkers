using Autofac;
using PierogiesBot.Persistence.GuildSettings.Features.Many;
using PierogiesBot.Persistence.GuildSettings.Features.Single;

namespace PierogiesBot.Persistence.GuildSettings.Features;

public static class ContainerBuilderExtensions
{
    public static void AddGuildSettingsMongoServices(this ContainerBuilder builder)
    {
        builder.RegisterType<SingleGuildSettingService>().AsImplementedInterfaces();
        builder.RegisterType<ManyGuildSettingsService>().AsImplementedInterfaces();
        builder.RegisterType<GuildSettingFacade>().AsImplementedInterfaces();
    }
}