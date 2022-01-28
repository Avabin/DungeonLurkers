using System.Reactive.Linq;
using System.Reactive.Subjects;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.Handlers;

namespace PierogiesBot.Discord.Infrastructure.Features.MessageSubscriptions.Handlers;

public class BotSubscriptionRuleMessageHandler : IRuleMessageHandler, IDisposable
{
    private readonly ISubject<SocketCommandContext>             _subject;
    private readonly IDisposable?                               _subDisposable;
    private readonly IEnumerable<IUserSocketMessageHandler>     _messageHandlers;
    private readonly ILogger<BotSubscriptionRuleMessageHandler> _logger;
    
    public IObservable<IResult> ResultObservable { get; }

    public BotSubscriptionRuleMessageHandler(ILogger<BotSubscriptionRuleMessageHandler> logger, IEnumerable<IUserSocketMessageHandler> messageHandlers)
    {
        _logger = logger;
        _messageHandlers = messageHandlers;
        _subject = new Subject<SocketCommandContext>();

        ResultObservable = _subject
            .Select(async x => await HandleMessageAsync(x))
            .Select(x => x.Result);
        
        _subDisposable = ResultObservable.Subscribe();
    }
    public void OnCompleted() => _subject.OnCompleted();

    public void OnError(Exception error) => _subject.OnError(error);

    public void OnNext(SocketCommandContext value) => _subject.OnNext(value);

    public void Dispose() => _subDisposable?.Dispose();

    private async ValueTask<IResult> HandleMessageAsync(SocketCommandContext messageContext)
    {
        try
        {
            foreach (var handler in _messageHandlers)
            {
                var result = await handler.HandleAsync(messageContext);
                if (result.IsSuccess)
                    return ExecuteResult.FromSuccess();
            }

            return ExecuteResult.FromError(CommandError.UnmetPrecondition,
                "There is no currently registered handler for that message");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while handling new message!");
            return ExecuteResult.FromError(e);
        }
    }
}