using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PierogiesBot.Discord.Commands.Features.React;

namespace PierogiesBot.Discord.Commands.Tests;

[TestFixture]
[Category("Unit")]
[Category("ReactionCommands")]
[TestOf(typeof(ReactionCommandModule))]
public class ReactionCommandModuleUnitTests
{
    private ReactionCommandModule _sut     = null!;
    private IGuild                _guild   = null!;
    private ICommandContext       _context = null!;
    private IMessageChannel       _channel = null!;


    [SetUp]
    public void Setup()
    {
        _sut     = new ReactionCommandModule(Substitute.For<ILogger<ReactionCommandModule>>());
        _guild   = Substitute.For<IGuild>();
        _context = Substitute.For<ICommandContext>();
        _channel = Substitute.For<IMessageChannel>();
    }


    [Test]
    public async Task When_React_WithCorrectReaction_Then_ReactionIsAddedToPreviousMessage()
    {
        // Arrange
        var message                   = Substitute.For<IUserMessage>();
        var previousMessage           = Substitute.For<IUserMessage>();
        var previousMessageEnumerable = new[] { previousMessage };
        var asyncPreviousMessages     = (previousMessageEnumerable as IReadOnlyCollection<IUserMessage>);
        var listAsyncEnumerable       = AsyncEnumerable.Range(0, 1).Select(_ => asyncPreviousMessages);


        var reaction = "lol";
        var emote    = CreateGuildEmote(reaction);
        var emotes   = new List<GuildEmote> { emote };
        
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _context.Message.Returns(message);
        _context.Channel.GetMessagesAsync(Arg.Is(message), Arg.Is(Direction.Before), Arg.Is(1))
                .Returns(listAsyncEnumerable);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.React(reaction);

        // Assert
        await previousMessage.Received(1).AddReactionAsync(emote);
    }
    
    [Test]
    public async Task When_React_WithWrongReaction_Then_NoReactionIsAddedToPreviousMessage()
    {
        // Arrange
        var message                   = Substitute.For<IUserMessage>();
        var previousMessage           = Substitute.For<IUserMessage>();
        var previousMessageEnumerable = new[] { previousMessage };
        var asyncPreviousMessages     = (previousMessageEnumerable as IReadOnlyCollection<IUserMessage>);
        var listAsyncEnumerable       = AsyncEnumerable.Range(0, 1).Select(_ => asyncPreviousMessages);


        var reaction = "lol";
        var emotes   = new List<GuildEmote> {  };
        
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _context.Message.Returns(message);
        _context.Channel.GetMessagesAsync(Arg.Is(message), Arg.Is(Direction.Before), Arg.Is(1))
                .Returns(listAsyncEnumerable);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.React(reaction);

        // Assert
        await previousMessage.DidNotReceive().AddReactionAsync(Arg.Any<IEmote>());
    }
    
    [Test]
    public async Task When_React_ToWrongMessage_Then_NoReactionIsAddedToPreviousMessage()
    {
        // Arrange
        var message                   = Substitute.For<IUserMessage>();
        var previousMessage           = Substitute.For<IUserMessage>();
        var previousMessageEnumerable = new IUserMessage[] { };
        var asyncPreviousMessages     = (previousMessageEnumerable as IReadOnlyCollection<IUserMessage>);
        var listAsyncEnumerable       = AsyncEnumerable.Range(0, 1).Select(_ => asyncPreviousMessages);

        var reaction = "lol";
        var emotes   = new List<GuildEmote> { CreateGuildEmote(reaction) };
        
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _context.Message.Returns(message);
        _context.Channel.GetMessagesAsync(Arg.Is(message), Arg.Is(Direction.Before), Arg.Is(1))
                .Returns(listAsyncEnumerable);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.React(reaction);

        // Assert
        await previousMessage.DidNotReceive().AddReactionAsync(Arg.Any<IEmote>());
    }
    
    [Test]
    public async Task When_React_ToSpecificMessage_Then_ReactionIsAddedToThatMessage()
    {
        // Arrange
        var messageId                 = 12345ul;
        var message                   = Substitute.For<IUserMessage>();

        var reaction = "lol";
        var emotes   = new List<GuildEmote> { CreateGuildEmote(reaction) };

        message.Id.Returns(messageId);
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _context.Message.Returns(message);
        _context.Channel.GetMessageAsync(Arg.Is(messageId)).Returns(message);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.React(messageId, reaction);

        // Assert
        await message.Received(1).AddReactionAsync(Arg.Is<IEmote>(x => x.Name.Equals(reaction)));
    }
    
    [Test]
    public async Task When_React_ToSpecificMessageWithWrongReaction_Then_NoReactionIsAddedToThatMessage()
    {
        // Arrange
        var messageId = 12345ul;
        var message   = Substitute.For<IUserMessage>();

        var reaction = "lol";
        var emotes   = new List<GuildEmote> { CreateGuildEmote("abc") };

        message.Id.Returns(messageId);
        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _context.Message.Returns(message);
        _context.Channel.GetMessageAsync(Arg.Is(messageId)).Returns(message);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.React(messageId, reaction);

        // Assert
        await message.DidNotReceive().AddReactionAsync(Arg.Any<IEmote>());
    }

    #region CreateGuildEmote

    private static Type[] _constructorParamsDefinition = new[]
    {
        typeof(ulong), typeof(string), typeof(bool), typeof(bool),
        typeof(bool), typeof(IReadOnlyList<ulong>), typeof(ulong?)
    };

    private static GuildEmote CreateGuildEmote(string reaction)
    {
        var constructorParamsValues = new object[] { 123ul, reaction, false, false, false, new List<ulong>(), null! };
        var emoteConstructor =
            typeof(GuildEmote).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                                              _constructorParamsDefinition, null);
        if (emoteConstructor == null)
            Assert.Fail("Could not find constructor for GuildEmote");
        var emote = (GuildEmote)emoteConstructor!.Invoke(constructorParamsValues);
        return emote;
    }

    #endregion
}