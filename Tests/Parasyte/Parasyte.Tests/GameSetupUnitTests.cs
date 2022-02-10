using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Parasyte.Features.GameSettings;
using Parasyte.Features.GameSetup;
using Parasyte.Features.GameSetup.Exceptions;
using Parasyte.Features.Players;

namespace Parasyte.Tests;

[TestFixture]
[TestOf(typeof(GamePlayers))]
[Category(nameof(Parasyte))]
[Category("Unit")]
public class GameSetupUnitTests
{
    private readonly GameSettings _settings = new GameSettings(990, 99, 99, 99, 99, 99);
    [Test]
    public void When_AddPlayer_Then_PlayerIsAdded()
    {
        // Arrange
        var sut    = new GamePlayers(_settings);
        var player = SetupPlayer();
        
        // Act
        sut.AddPlayer(player);
        
        // Assert
        sut.Players.Should().Contain(player);
    }
    
    [Test]
    public void When_AddPlayer_WithUnavailableRole_Then_ExceptionIsThrown()
    {
        // Arrange
        var sut    = new GamePlayers(_settings with {MaxDoctorCount = 0});
        var player = SetupPlayer("doctor", PlayerRole.Doctor);
        
        // Act
        var act = () => sut.AddPlayer(player);
        
        // Assert
        act.Should().Throw<TooManyRolePlayersException>(because: "Doctor role is unavailable").Where(x => x.Role == PlayerRole.Doctor);
    }
    
    [Test]
    public void When_AddPlayer_WhenMaxPlayersReached_Then_ExceptionIsThrown()
    {
        // Arrange
        var sut    = new GamePlayers(_settings with {MaxPlayerCount = 0});
        var player = SetupPlayer();
        
        // Act
        var act = () => sut.AddPlayer(player);
        
        // Assert
        act.Should().Throw<TooManyPlayersException>(because: "Max players reached").Where(x => x.MaxPlayers == 0);
    }
    
    [Test]
    public void When_AddPlayers_Then_PlayersAreAdded()
    {
        // Arrange
        var sut     = new GamePlayers(_settings);
        var players = Enumerable.Range(0, 10).Select(_ => SetupPlayer()).ToList();
        
        // Act
        sut.AddPlayers(players);
        
        // Assert
        sut.Players.Should().BeEquivalentTo(players);
    }
    
    [Test]
    public void When_RemovePlayer_Then_PlayerIsRemoved()
    {
        // Arrange
        var sut    = new GamePlayers(_settings);
        var player = SetupPlayer();
        
        sut.AddPlayer(player);
        // Act
        sut.RemovePlayer(player);
        
        // Assert
        sut.Players.Should().NotContain(player);
    }
    
    [Test]
    public void When_RemovePlayerNotInGame_Then_ExceptionIsThrown()
    {
        // Arrange
        var sut    = new GamePlayers(_settings);
        var player = SetupPlayer();
        var playerTwo = SetupPlayer();
        
        sut.AddPlayer(player);
        // Act
        var act = () => sut.RemovePlayer(playerTwo);
        
        // Assert
        act.Should().Throw<PlayerNotInGameException>().Where(x => x.Player == playerTwo);
    }
    
    [Test]
    public void When_RemovePlayers_Then_PlayersAreRemoved()
    {
        // Arrange
        var sut     = new GamePlayers(_settings);
        var players = Enumerable.Range(0, 10).Select(_ => SetupPlayer()).ToList();
        
        sut.AddPlayers(players);
        // Act
        sut.RemovePlayers(players);
        
        // Assert
        sut.Players.Should().NotContain(players);
        
    }

    private static IPlayer SetupPlayer(string name = "player")
    {
        var random = Random.Shared;
        var mock   = Substitute.For<IPlayer>();
        var roles  = Enum.GetValues<PlayerRole>();
        mock.PlayerRole.Returns(roles[random.Next(roles.Length)]);
        mock.Name.Returns(name);
        return mock;
    }
    
    private static IPlayer SetupPlayer(string name, PlayerRole role)
    {
        var mock   = Substitute.For<IPlayer>();
        mock.PlayerRole.Returns(role);
        mock.Name.Returns(name);
        return mock;
    }
}