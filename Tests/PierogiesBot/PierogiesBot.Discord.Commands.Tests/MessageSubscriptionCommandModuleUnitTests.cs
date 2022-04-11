using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PierogiesBot.Discord.Commands.Features.MessageSubscriptions;
using PierogiesBot.Discord.Core.Features.MessageSubscriptions.SubscriptionServices;

namespace PierogiesBot.Discord.Commands.Tests;

[TestFixture]
[Category("Unit")]
[Category("SubscribeCommands")]
[Category("PierogiesBot")]
[TestOf(typeof(SubscribeCommandModule))]
public class SubscribeCommandModuleUnitTests
{
    private SubscribeCommandModule.SubscribeResponsesCommandModule _subscribeResponsesCommandModule = null!;
    private SubscribeCommandModule.SubscribeCrontabCommandModule   _subscribeCrontabCommandModule   = null!;
    
    private IChannelSubscribeService _channelSubscribeService = null!;
    private ICrontabSubscribeService _crontabSubscribeService = null!;
    
    private IGuild          _guild    = null!;
    private ICommandContext _context  = null!;
    private IMessageChannel _channel  = null!;

    [SetUp]
    public void SetUp()
    {
        _channelSubscribeService         = Substitute.For<IChannelSubscribeService>();
        _crontabSubscribeService         = Substitute.For<ICrontabSubscribeService>();

        _subscribeResponsesCommandModule = new SubscribeCommandModule.SubscribeResponsesCommandModule(Substitute.For<ILogger<SubscribeCommandModule.SubscribeResponsesCommandModule>>(), _channelSubscribeService);
        _subscribeCrontabCommandModule   = new SubscribeCommandModule.SubscribeCrontabCommandModule(Substitute.For<ILogger<SubscribeCommandModule.SubscribeCrontabCommandModule>>(), _crontabSubscribeService);
        
        _guild = Substitute.For<IGuild>();
        _context = Substitute.For<ICommandContext>();
        _channel = Substitute.For<IMessageChannel>();
    }

    #region Responses

    [Test]
    public async Task When_SubscribeToAll_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 10)
                           .Select(i => Substitute.For<ITextChannel>())
                           .ToList();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetTextChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeResponsesCommandModule).SetContext(_context);
        // Act
        await _subscribeResponsesCommandModule.Subscribe();
        
        // Assert
        await _channelSubscribeService.Received(guildChannels.Count).SubscribeAsync(Arg.Is<IGuildChannel>(x => guildChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task When_Subscribe_TiSpecificChannel_Then_SubscribeServiceIsCalledForThatChannel_And_Replies()
    {
        // Arrange
        var guildChannel = Substitute.For<IGuildChannel>();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        
        ((IModuleBase)_subscribeResponsesCommandModule).SetContext(_context);
        // Act
        await _subscribeResponsesCommandModule.Subscribe(guildChannel);
        
        // Assert
        await _channelSubscribeService.Received(1).SubscribeAsync(Arg.Is(guildChannel));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task When_SubscribeToChannels_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 20)
                           .Select(i => Substitute.For<IGuildChannel>())
                           .ToList();
        var subscribedChannels = guildChannels.Skip(5).Take(15).ToArray();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeResponsesCommandModule).SetContext(_context);
        // Act
        await _subscribeResponsesCommandModule.Subscribe(subscribedChannels.ToArray());
        
        // Assert
        await _channelSubscribeService.Received(subscribedChannels.Length).SubscribeAsync(Arg.Is<IGuildChannel>(x => subscribedChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task When_UnsubscribeFromAll_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 10)
                           .Select(i => Substitute.For<ITextChannel>())
                           .ToList();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetTextChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeResponsesCommandModule).SetContext(_context);
        // Act
        await _subscribeResponsesCommandModule.Unsubscribe();
        
        // Assert
        await _channelSubscribeService.Received(guildChannels.Count).UnsubscribeAsync(Arg.Is<IGuildChannel>(x => guildChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task When_Unsubscribe_TiSpecificChannel_Then_SubscribeServiceIsCalledForThatChannel_And_Replies()
    {
        // Arrange
        var guildChannel = Substitute.For<IGuildChannel>();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        
        ((IModuleBase)_subscribeResponsesCommandModule).SetContext(_context);
        // Act
        await _subscribeResponsesCommandModule.Unsubscribe(guildChannel);
        
        // Assert
        await _channelSubscribeService.Received(1).UnsubscribeAsync(Arg.Is(guildChannel));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task When_UnsubscribeToChannels_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 20)
                           .Select(i => Substitute.For<IGuildChannel>())
                           .ToList();
        var unsubscribedChannels = guildChannels.Skip(5).Take(15).ToArray();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeResponsesCommandModule).SetContext(_context);
        // Act
        await _subscribeResponsesCommandModule.Unsubscribe(unsubscribedChannels.ToArray());
        
        // Assert
        await _channelSubscribeService.Received(unsubscribedChannels.Length).UnsubscribeAsync(Arg.Is<IGuildChannel>(x => unsubscribedChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }

    #endregion

    #region Crontab

    [Test]
    public async Task WhenCrontab_SubscribeToAll_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 10)
                           .Select(i => Substitute.For<IGuildChannel>())
                           .ToList();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeCrontabCommandModule).SetContext(_context);
        // Act
        await _subscribeCrontabCommandModule.Subscribe();
        
        // Assert
        await _crontabSubscribeService.Received(guildChannels.Count).SubscribeAsync(Arg.Is<IGuildChannel>(x => guildChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task WhenCrontab_Subscribe_TiSpecificChannel_Then_SubscribeServiceIsCalledForThatChannel_And_Replies()
    {
        // Arrange
        var guildChannel = Substitute.For<IGuildChannel>();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        
        ((IModuleBase)_subscribeCrontabCommandModule).SetContext(_context);
        // Act
        await _subscribeCrontabCommandModule.Subscribe(guildChannel);
        
        // Assert
        await _crontabSubscribeService.Received(1).SubscribeAsync(Arg.Is(guildChannel));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task WhenCrontab_SubscribeToChannels_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 20)
                           .Select(i => Substitute.For<IGuildChannel>())
                           .ToList();
        var subscribedChannels = guildChannels.Skip(5).Take(15).ToArray();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeCrontabCommandModule).SetContext(_context);
        // Act
        await _subscribeCrontabCommandModule.Subscribe(subscribedChannels.ToArray());
        
        // Assert
        await _crontabSubscribeService.Received(subscribedChannels.Length).SubscribeAsync(Arg.Is<IGuildChannel>(x => subscribedChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task WhenCrontab_UnsubscribeFromAll_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 10)
                           .Select(i => Substitute.For<IGuildChannel>())
                           .ToList();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeCrontabCommandModule).SetContext(_context);
        // Act
        await _subscribeCrontabCommandModule.Unsubscribe();
        
        // Assert
        await _crontabSubscribeService.Received(guildChannels.Count).UnsubscribeAsync(Arg.Is<IGuildChannel>(x => guildChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task WhenCrontab_Unsubscribe_TiSpecificChannel_Then_SubscribeServiceIsCalledForThatChannel_And_Replies()
    {
        // Arrange
        var guildChannel = Substitute.For<IGuildChannel>();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        
        ((IModuleBase)_subscribeCrontabCommandModule).SetContext(_context);
        // Act
        await _subscribeCrontabCommandModule.Unsubscribe(guildChannel);
        
        // Assert
        await _crontabSubscribeService.Received(1).UnsubscribeAsync(Arg.Is(guildChannel));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }
    
    [Test]
    public async Task WhenCrontab_UnsubscribeToChannels_Then_SubscribeServiceIsCalledForEachChannel_And_Replies()
    {
        // Arrange
        var guildChannels = Enumerable
                           .Range(1, 20)
                           .Select(i => Substitute.For<IGuildChannel>())
                           .ToList();
        var unsubscribedChannels = guildChannels.Skip(5).Take(15).ToArray();
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _guild.GetChannelsAsync().Returns(guildChannels);
        
        ((IModuleBase)_subscribeCrontabCommandModule).SetContext(_context);
        // Act
        await _subscribeCrontabCommandModule.Unsubscribe(unsubscribedChannels.ToArray());
        
        // Assert
        await _crontabSubscribeService.Received(unsubscribedChannels.Length).UnsubscribeAsync(Arg.Is<IGuildChannel>(x => unsubscribedChannels.Contains(x)));
        await _channel.Received(1).SendMessageAsync(Arg.Any<string>());
    }

    #endregion


}