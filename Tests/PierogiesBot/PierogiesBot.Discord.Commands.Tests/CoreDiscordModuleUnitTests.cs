using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PierogiesBot.Discord.Commands.Features;
using PierogiesBot.Discord.Infrastructure.Features;

namespace PierogiesBot.Discord.Commands.Tests;

[TestFixture]
[Category("Unit")]
[Category("CoreCommands")]
[Category("PierogiesBot")]
[TestOf(typeof(CoreDiscordModule))]
public class CoreDiscordModuleUnitTests
{
    private CoreDiscordModule _sut;
    [SetUp]
    public void Setup()
    {
        _sut = new CoreDiscordModule(Substitute.For<ILogger<CoreDiscordModule>>());
    }

    [Test]
    public async Task When_Ping_Then_Pong()
    {
        // Arrange
        var context = Substitute.For<ICommandContext>();
        var channel = Substitute.For<IMessageChannel>();
        var message = Substitute.For<IUserMessage>();
        
        context.Channel.Returns(channel);
        channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(message));

        ((IModuleBase)_sut).SetContext(context);
        
        // Act
        await _sut.Ping();
        
        // Assert
        await channel.Received(1).SendMessageAsync(Arg.Is<string>(x => x.Contains("Pong", StringComparison.InvariantCultureIgnoreCase)));
    }
}