using Autofac;
using Microsoft.Extensions.Configuration;
using PierogiesBot.Discord.Infrastructure;
using PierogiesBot.Discord.Infrastructure.Features.DiscordHost;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Persistence.BotReactRules.Features;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Persistence.GuildSettings.Features;
using Shared.MessageBroker.RabbitMQ;
using Shared.Persistence.Core.Features;
using Shared.Persistence.Mongo.Features;

namespace PierogiesBot.Infrastructure;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddInfrastructure(this ContainerBuilder builder, IConfiguration configuration)
    {
        if(configuration["IsDiscordEnabled"] == bool.TrueString)
            builder.AddDiscordServices<DiscordService>();
        builder.AddPersistenceCore();
        builder.AddPersistenceMongo();
        if (configuration["Rabbit:IsEnabled"] == bool.TrueString)
            builder.AddRabbitMqMessageBroker();
        else
            builder.AddInternalMessageBroker();
        builder.AddBotCrontabRulesMongoServices();
        builder.AddBotReactRulesMongoServices();
        builder.AddBotMessageSubscriptionsMongoServices();
        builder.AddBotResponseRulesMongoServices();
        builder.AddGuildSettingsMongoServices();
        
        return builder;
    } 
}