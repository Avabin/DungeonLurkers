// using System;
// using System.Threading.Tasks;
// using Discord;
// using Discord.Commands;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NSubstitute.ReturnsExtensions;
// using NUnit.Framework;
// using PierogiesBot.Discord.Commands.Features.GuildSettings;
// using PierogiesBot.Persistence.GuildSettings.Features;
// using TimeZoneConverter;
//
// namespace PierogiesBot.Discord.Commands.Tests;
//
// [TestFixture]
// [Category("Unit")]
// [Category("GuildSettingsCommands")]
// [Category("PierogiesBot")]
// [TestOf(typeof(GuildSettingsCommandModule))]
// public class GuildSettingsCommandModuleUnitTests
// {
//     private GuildSettingsCommandModule _sut    = null!;
//     private IGuildSettingFacade        _facade = null!;
//
//     private TimeZoneInfo    _tzInfo   = null!;
//     private IGuild          _guild    = null!;
//     private ICommandContext _context  = null!;
//     private IMessageChannel _channel  = null!;
//     private IUserMessage    _message  = null!;
//     private IRole           _muteRole = null!;
//
//     [SetUp]
//     public void Setup()
//     {
//         _facade = Substitute.For<IGuildSettingFacade>();
//         _sut    = new GuildSettingsCommandModule(Substitute.For<ILogger<GuildSettingsCommandModule>>(), _facade);
//         
//         _tzInfo   = TZConvert.GetTimeZoneInfo("Europe/Warsaw");
//         _guild    = Substitute.For<IGuild>();
//         _context  = Substitute.For<ICommandContext>();
//         _channel  = Substitute.For<IMessageChannel>();
//         _message  = Substitute.For<IUserMessage>();
//         _muteRole = Substitute.For<IRole>();
//     }
//
//     [Test]
//     public async Task When_SetTimezone_WithCorrectTimezone_Then_SetsTimezoneInSettings_And_Replies()
//     {
//         // Arrange
//         _guild.Id.Returns(123ul);
//         _context.Guild.Returns(_guild);
//         _context.Channel.Returns(_channel);
//
//         _facade.SetGuildTimezoneAsync(Arg.Is(_tzInfo.Id), Arg.Any<ulong>()).Returns(Task.CompletedTask);
//         _channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(_message));
//
//         ((IModuleBase)_sut).SetContext(_context);
//         // Act
//         await _sut.SetTimeZone(_tzInfo);
//
//         // Assert
//         await _facade.Received(1).SetGuildTimezoneAsync(Arg.Is(_tzInfo.Id), Arg.Any<ulong>());
//         await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains(_tzInfo.ToString())));
//     }
//
//     [Test]
//     public async Task When_GetTimezone_AndGuildHasTimezone_Then_GetsTimezoneInSettings_And_Replies()
//     {
//         // Arrange
//         var guildId = 123ul;
//
//         _guild.Id.Returns(guildId);
//         _context.Guild.Returns(_guild);
//         _context.Channel.Returns(_channel);
//
//         _facade.GetGuildTimezoneAsync(Arg.Is(guildId)).Returns(_tzInfo.Id);
//         _channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(_message));
//
//         ((IModuleBase)_sut).SetContext(_context);
//         // Act
//         await _sut.GetTimeZone();
//
//         // Assert
//         await _facade.Received(1).GetGuildTimezoneAsync(Arg.Is(guildId));
//         await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains(_tzInfo.Id)));
//     }
//
//     [Test]
//     public async Task When_GetTimezone_AndGuildDoesNotHasTimezone_Then_Replies_NothingFound()
//     {
//         // Arrange
//         var guildId = 123ul;
//
//         _guild.Id.Returns(guildId);
//         _context.Guild.Returns(_guild);
//         _context.Channel.Returns(_channel);
//
//         _facade.GetGuildTimezoneAsync(Arg.Is(guildId)).ReturnsNull();
//         _channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(_message));
//
//         ((IModuleBase)_sut).SetContext(_context);
//         // Act
//         await _sut.GetTimeZone();
//
//         // Assert
//         await _facade.Received(1).GetGuildTimezoneAsync(Arg.Is(guildId));
//         await _channel.Received(1).SendMessageAsync(Arg.Is("Nothing found"));
//     }
//
//     [Test]
//     public async Task When_SetMuteRole_WithCorrectMuteRole_Then_SetsMuteRoleInSettings_And_Replies()
//     {
//         // Arrange
//         var muteRoleId = 123ul;
//         var guildId   = 1234ul;
//
//         _muteRole.Id.Returns(muteRoleId);
//         _guild.Id.Returns(guildId);
//         _context.Guild.Returns(_guild);
//         _context.Channel.Returns(_channel);
//
//         _facade.SetMuteRoleAsync(Arg.Is(guildId), Arg.Is(muteRoleId)).Returns(Task.CompletedTask);
//         _channel.SendMessageAsync(Arg.Is<string>(s => s.Contains(muteRoleId.ToString()))).ReturnsForAnyArgs(Task.FromResult(_message));
//
//         ((IModuleBase)_sut).SetContext(_context);
//         // Act
//         await _sut.SetMuteRole(_muteRole);
//
//         // Assert
//         await _facade.Received(1).SetMuteRoleAsync(Arg.Is(guildId), Arg.Is(muteRoleId));
//         await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains(_muteRole.ToString())));
//     }
//
//     [Test]
//     public async Task When_GetMuteRole_AndGuildHasMuteRole_Then_GetsMuteRoleInSettings_And_Replies()
//     {
//         // Arrange
//         var guildId = 123ul;
//         var muteRoleId = 1234ul;
//
//         _guild.Id.Returns(guildId);
//         _context.Guild.Returns(_guild);
//         _context.Channel.Returns(_channel);
//
//         _facade.GetMuteRoleAsync(Arg.Is(guildId)).Returns(muteRoleId);
//         _channel.SendMessageAsync(Arg.Is<string>(s => s.Contains(muteRoleId.ToString()))).ReturnsForAnyArgs(Task.FromResult(_message));
//
//         ((IModuleBase)_sut).SetContext(_context);
//         // Act
//         await _sut.GetMuteRole();
//
//         // Assert
//         await _facade.Received(1).GetMuteRoleAsync(Arg.Is(guildId));
//         await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains(muteRoleId.ToString())));
//     }
//
//     [Test]
//     public async Task When_GetMuteRole_AndGuildDoesNotHasMuteRoleThen_Replies_ThereIsNoMuteRoleSet()
//     {
//         // Arrange
//         var guildId = 123ul;
//
//         _guild.Id.Returns(guildId);
//         _context.Guild.Returns(_guild);
//         _context.Channel.Returns(_channel);
//
//         _facade.GetMuteRoleAsync(Arg.Is(guildId)).Returns(0ul);
//         _channel.SendMessageAsync(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(_message));
//
//         ((IModuleBase)_sut).SetContext(_context);
//         // Act
//         await _sut.GetMuteRole();
//
//         // Assert
//         await _facade.Received(1).GetMuteRoleAsync(Arg.Is(guildId));
//         await _channel.Received(1).SendMessageAsync(Arg.Is<string>(s => s.Contains("There is no mute role set")));
//     }
// }