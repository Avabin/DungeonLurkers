using Microsoft.Extensions.Hosting;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using Shared.MessageBroker.Core;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Crontab;

public class CrontabChangesReceiver : DocumentChangesAwareBase<BotCrontabRuleDocument>, IHostedService
{
    private readonly ICrontabGuildService _guildService;

    public CrontabChangesReceiver(ICrontabGuildService guildService, IDocumentMessageBroker messageBroker) : base(messageBroker)
    {
        _guildService = guildService;
    }

    protected override async Task HandleDocumentUpdatedAsync(BotCrontabRuleDocument oldDocument, BotCrontabRuleDocument updatedDocument) => 
        await _guildService.RescheduleJobsForRule(updatedDocument);

    protected override async Task HandleDocumentDeletedAsync(BotCrontabRuleDocument deletedDocument) =>
        await _guildService.RemoveJobsForRule(deletedDocument.Id);

    protected override async Task HandleDocumentInsertedAsync(BotCrontabRuleDocument newDocument)
    {
        await _guildService.ScheduleJobsForRule(newDocument);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken  cancellationToken)
    {
        Stop();
        return Task.CompletedTask;
    }
}