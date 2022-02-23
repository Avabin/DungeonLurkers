using System.Reactive.Linq;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PierogiesBot.Discord.Commands.Features;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions;
using PierogiesBot.Discord.Core.Features.TimeZoneTypeConverter;
using PierogiesBot.Discord.Interactions.Features.WhoIs;
using IResult = Discord.Commands.IResult;
using RunMode = Discord.Interactions.RunMode;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

// ReSharper disable ContextualLoggerProblem

namespace PierogiesBot.Discord.Infrastructure.Features.DiscordHost;

public class DiscordService : IDiscordService
{
    private readonly IServiceProvider                _services;
    private readonly DiscordSocketClient             _client;
    private readonly CommandService                  _commandService;
    private readonly ILogger<CommandService>         _commandLogger;
    private readonly ILogger<InteractionService>     _interactionLogger;
    private readonly ILogger<DiscordSocketClient>    _discordLogger;
    private readonly IOptions<DiscordSettings>       _settings;
    private readonly IEnumerable<ILoadSubscriptions> _subscriptions;
    private readonly InteractionService              _interactionService;

    public DiscordService(
        IServiceProvider                services,
        ILogger<CommandService>         commandLogger,
        ILogger<InteractionService>         interactionLogger,
        ILogger<DiscordSocketClient>    discordLogger,
        IOptions<DiscordSettings>       settings,
        IOptions<CommandServiceConfig>  commandConfig,
        IEnumerable<ILoadSubscriptions> subscriptions,
        DiscordSocketClient client)
    {
        _services               = services;
        _commandLogger          = commandLogger;
        _interactionLogger = interactionLogger;
        _discordLogger          = discordLogger;
        _settings               = settings;
        _subscriptions          = subscriptions;

        _client         = client;
        _commandService = new CommandService(new CommandServiceConfig
        {
            DefaultRunMode = global::Discord.Commands.RunMode.Async
        });
        
        _interactionService = new InteractionService(_client, new InteractionServiceConfig
        {
            UseCompiledLambda = true,
            DefaultRunMode    = RunMode.Async,
        });

        MessageObservable = Observable.FromEvent<Func<SocketMessage, Task>, SocketMessage>(
         h => _client.MessageReceived += h,
         h => _client.MessageReceived -= h);
    }
    public IObservable<SocketMessage> MessageObservable                        { get; }

    public SocketGuild? GetGuild(ulong guildId) => _client.Guilds.FirstOrDefault(g => g.Id == guildId);

    public SocketGuildChannel? GetChannel(ulong channelId, ulong guildId) =>
        _client.Guilds.FirstOrDefault(g => g.Id   == guildId)
              ?.Channels.FirstOrDefault(c => c.Id == channelId);

    public async Task Start()
    {
        var eventAwaiter = new TaskCompletionSource<bool>(false);

        _client.Log += message =>
        {
            var logLevel = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error    => LogLevel.Error,
                LogSeverity.Warning  => LogLevel.Warning,
                LogSeverity.Info     => LogLevel.Information,
                LogSeverity.Verbose  => LogLevel.Debug,
                LogSeverity.Debug    => LogLevel.Trace,
                _                    => throw new ArgumentOutOfRangeException(nameof(message), "has wrong LogSeverity!")
            };
            if (message.Exception is not null) _discordLogger.LogError(message.Exception, message.Message);
            else _discordLogger.Log(logLevel, message.Message);
            return Task.CompletedTask;
        };

        await _client.LoginAsync(TokenType.Bot, _settings.Value.Token).ConfigureAwait(false);
        await _client.StartAsync().ConfigureAwait(false);
        _client.Ready += () =>
        {
            eventAwaiter.SetResult(true);
            return Task.CompletedTask;
        };

        await eventAwaiter.Task.ConfigureAwait(false);
        await InstallCommandsAsync();
        await InstallInteractionsAsync();

        await Task.WhenAll(_subscriptions.Select(s => s.LoadSubscriptionsAsync()));
    }

    private async Task InstallInteractionsAsync()
    {
        _interactionLogger.LogInformation("Installing interactions...");
        _interactionService.Log += message =>
        {
            var logLevel = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error    => LogLevel.Error,
                LogSeverity.Warning  => LogLevel.Warning,
                LogSeverity.Info     => LogLevel.Information,
                LogSeverity.Verbose  => LogLevel.Debug,
                LogSeverity.Debug    => LogLevel.Trace,
                _                    => throw new ArgumentOutOfRangeException(nameof(message), "has wrong LogSeverity!")
            };
            if (message.Exception is not null) _interactionLogger.LogError(message.Exception, message.Message);
            else _interactionLogger.Log(logLevel, message.Message);
            return Task.CompletedTask;
        };
        
        _interactionService.AddTypeConverter(typeof(TimeZoneInfo), new TimeZoneInfoConverter());
        await _interactionService.AddModulesAsync(typeof(WhoIsInteractionModule).Assembly, _services);

        var guilds = _client.Guilds;

        await Task.WhenAll(guilds.Select(g =>
        {
            _interactionLogger.LogDebug("Installing interactions for guild {Guild}...", g);
            return _interactionService.RegisterCommandsToGuildAsync(g.Id);
        }));
        
        _client.InteractionCreated += async interaction =>
        {
            _interactionLogger.LogTrace("Interaction created: {InteractionId}", interaction.Id);
            var scope = _services.CreateScope();
            var ctx   = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
        };
    }

    public async Task Stop()
    {
        _client.MessageReceived         -= HandleCommandAsync;
        _commandService.CommandExecuted -= CommandServiceOnCommandExecuted;

        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    private async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived         += HandleCommandAsync;
        _commandService.CommandExecuted += CommandServiceOnCommandExecuted;

        _commandService.AddTypeReader(typeof(TimeZoneInfo), new TimeZoneInfoTypeReader());

        // Here we discover all of the command modules in the entry 
        // assembly and load them. Starting from Discord.NET 2.0, a
        // service provider is required to be passed into the
        // module registration method to inject the 
        // required dependencies.
        //
        // If you do not use Dependency Injection, pass null.
        // See Dependency Injection guide for more information.
        await _commandService.AddModulesAsync(
                                              assembly: Assembly.GetAssembly(typeof(CoreDiscordModule)),
                                              services: _services);
    }

    private Task CommandServiceOnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext ctx,
                                                 IResult               result)
    {
        if (!commandInfo.IsSpecified) return Task.CompletedTask;

        var cmd = commandInfo.Value!;

        if (result.IsSuccess)
            _commandLogger.LogTrace("<{Guild}|>{Channel}|{User}>{CommandName}", ctx.Guild, ctx.Channel, ctx.User,
                                    cmd.Name);
        else
            _commandLogger.LogError(result.ErrorReason);

        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        if (messageParam is not SocketUserMessage message) return;

        // Create a number to track where the prefix ends and the command begins
        var argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('\\', ref argPos) ||
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commandService.ExecuteAsync(
                                           context: context,
                                           argPos: argPos,
                                           services: _services);
    }
}