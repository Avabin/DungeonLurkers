using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PierogiesBot.Discord.Commands.Features.CheckUser;

namespace PierogiesBot.Discord.Commands.Tests;

[TestFixture]
[Category("Unit")]
[Category("CheckUserCommands")]
[TestOf(typeof(CheckUserCommandModule))]
public class CheckUserCommandModuleUnitTests
{
    private CheckUserCommandModule _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new CheckUserCommandModule(Substitute.For<ILogger<CheckUserCommandModule>>());
    }

    [Test]
    public async Task When_Whois_WithoutUser_Then_Replies_With_CurrentUserDetails()
    {
        // Arrange
        var client  = Substitute.For<IDiscordClient>();
        var user = Substitute.For<ISelfUser>();
        var context = Substitute.For<ICommandContext>();
        var channel = Substitute.For<IMessageChannel>();
        var message = Substitute.For<IUserMessage>();

        var username = "TestUser";
        var discriminator = "1234";

        context.Client.Returns(client);
        context.Channel.Returns(channel);
        client.CurrentUser.Returns(user);
        user.Username.Returns(username);
        user.Discriminator.Returns(discriminator);

        channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(message);

        ((IModuleBase)_sut).SetContext(context);

        // Act
        await _sut.WhoIs();

        // Assert
        await channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains(username) && s.Contains(discriminator)));
    }
    
    [Test]
    public async Task When_Whois_WithUser_Then_Replies_With_UserDetails()
    {
        // Arrange
        var user    = Substitute.For<ISelfUser>();
        var context = Substitute.For<ICommandContext>();
        var channel = Substitute.For<IMessageChannel>();
        var message = Substitute.For<IUserMessage>();

        var username      = "TestUser";
        var discriminator = "1234";

        context.Channel.Returns(channel);
        user.Username.Returns(username);
        user.Discriminator.Returns(discriminator);

        channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(message);

        ((IModuleBase)_sut).SetContext(context);

        // Act
        await _sut.WhoIs(user);

        // Assert
        await channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains(username) && s.Contains(discriminator)));
    }
}