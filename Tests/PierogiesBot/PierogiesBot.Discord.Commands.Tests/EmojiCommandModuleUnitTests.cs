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
using PierogiesBot.Discord.Commands.Features.SendEmoji;

namespace PierogiesBot.Discord.Commands.Tests;

[TestFixture]
[Category("Unit")]
[Category("EmojiCommands")]
[Category("PierogiesBot")]
[TestOf(typeof(EmojiCommandModule))]
public class EmojiCommandModuleUnitTests
{
    private EmojiCommandModule _sut     = null!;
    private IGuild             _guild   = null!;
    private ICommandContext    _context = null!;
    private IMessageChannel    _channel = null!;


    [SetUp]
    public void Setup()
    {
        _sut     = new EmojiCommandModule(Substitute.For<ILogger<EmojiCommandModule>>());
        _guild   = Substitute.For<IGuild>();
        _context = Substitute.For<ICommandContext>();
        _channel = Substitute.For<IMessageChannel>();
    }
    
    [Test]
    public async Task When_Emoji_WithCorrectEmoji_Then_NewMessageWithThatEmojiIsSent()
    {
        // Arrange
        var message = Substitute.For<IUserMessage>();

        var emoji  = "lol";
        var emote  = CreateGuildEmote(emoji);
        var emotes = new List<GuildEmote> { emote };

        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);
        _context.Message.Returns(message);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.Emoji(emoji);

        // Assert
        await _channel.Received(1).SendMessageAsync(Arg.Is<string>(x => x.Contains(emoji)));
    }

    [Test]
    public async Task When_Emojis_WithCorrectEmojis_Then_NewMessageWithThatEmojisIsSent()
    {
        // Arrange
        var emojis = Enumerable.Range(0, 10).Select(i => $"Emoji{i}").ToArray();
        var emotes = emojis.Select(CreateGuildEmote).ToArray();

        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.Emojis(emojis.ToArray());

        // Assert
        await _channel.Received(1).SendMessageAsync(Arg.Is<string>(x => emotes.All(y => x.Contains(y.ToString()))));
    }

    [Test]
    public async Task When_Emoji_WithWrongEmoji_Then_ErrorMessageIsSent()
    {
        // Arrange
        var emoji  = $"Emoji1";
        var emote  = CreateGuildEmote(emoji);
        var emotes = new List<GuildEmote> { emote };

        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.Emoji("Emoji2");

        // Assert
        await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => !s.Contains(emote.ToString())));
    }

    [Test]
    public async Task When_Emojis_WithWrongEmojis_Then_ErrorMessageIsSent()
    {
        // Arrange
        var emojis = Enumerable.Range(0, 10).Select(i => $"Emoji{i}");
        var emotes = emojis.Select(CreateGuildEmote).ToList();

        _context.Guild.Returns(_guild);
        _context.Channel.Returns(_channel);

        _guild.Emotes.Returns(emotes);

        ((IModuleBase)_sut).SetContext(_context);
        // Act
        await _sut.Emojis("1", "2", "3");

        // Assert
        await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains("Not found", StringComparison.InvariantCultureIgnoreCase)));
    }

    #region CreateGuildEmote

    private static readonly Type[] _constructorParamsDefinition = new[]
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