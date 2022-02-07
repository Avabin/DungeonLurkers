using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Parasyte.Features.GameSetup;
using Parasyte.Features.Players;
using Parasyte.Features.Voting;

namespace Parasyte.Tests;

[TestFixture]
[Category(nameof(Parasyte))]
[TestOf(typeof(VotingFacade))]
[Category("Unit")]
public class VotingFacadeTests
{
    [Test]
    public void When_StartVote_NewVotingIsCreated()
    {
        // Arrange
        var gamePlayers = Substitute.For<IGamePlayers>();
        var voting = Substitute.For<IVoting>();
        var votingFactory = Substitute.For<IVotingFactory>();
        var sut    = new VotingFacade(gamePlayers, votingFactory);

        votingFactory.CreateVoting().Returns(voting);
        
        // Act
        sut.StartVote();
        
        // Assert
        votingFactory.Received(1).CreateVoting();
        sut.CurrentVoting.Should().Be(voting);
    }

    [Test]
    public void When_AddVote_And_PlayersInVoters_Then_VoteIsAddedToVoting()
    {
        // Arrange
        var gamePlayers = Substitute.For<IGamePlayers>();
        var voting = Substitute.For<IVoting>();
        var votingFactory = Substitute.For<IVotingFactory>();
        var players = Enumerable.Range(1, 2).Select(i => Substitute.For<IPlayer>()).ToList();
        var sut    = new VotingFacade(gamePlayers, votingFactory);
        var expected = new Vote(players[0], players[1]);

        votingFactory.CreateVoting().Returns(voting);
        gamePlayers.Players.Returns(players);
        
        // Act
        sut.StartVote();
        sut.AddVote(players[0], players[1]);
        
        // Assert
        voting.Received(1).AddVote(expected);
    }
    
    [Test]
    public void When_AddVote_And_PlayersNotInVoters_Then_ExceptionIsThrown()
    {
        // Arrange
        var gamePlayers   = Substitute.For<IGamePlayers>();
        var voting        = Substitute.For<IVoting>();
        var votingFactory = Substitute.For<IVotingFactory>();
        var players       = Enumerable.Range(1, 2).Select(i => Substitute.For<IPlayer>()).ToList();
        var sut           = new VotingFacade(gamePlayers, votingFactory);

        votingFactory.CreateVoting().Returns(voting);
        gamePlayers.Players.Returns(new List<IPlayer>());
        
        // Act
        sut.StartVote();
        var act = () => sut.AddVote(players[0], players[1]);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void When_AddVote_And_VoteNotStarted_Then_ExceptionIsThrown()
    {
        // Arrange
        var gamePlayers   = Substitute.For<IGamePlayers>();
        var voting        = Substitute.For<IVoting>();
        var votingFactory = Substitute.For<IVotingFactory>();
        var players       = Enumerable.Range(1, 2).Select(i => Substitute.For<IPlayer>()).ToList();
        var sut           = new VotingFacade(gamePlayers, votingFactory);

        votingFactory.CreateVoting().Returns(voting);
        gamePlayers.Players.Returns(new List<IPlayer>());
        
        // Act
        var act = () => sut.AddVote(players[0], players[1]);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Test]
    public void When_FinishVote_And_VotingStarted_Then_VoteResultIsReturned()
    {
        // Arrange
        var gamePlayers   = Substitute.For<IGamePlayers>();
        var voting        = Substitute.For<IVoting>();
        var votingFactory = Substitute.For<IVotingFactory>();
        var sut           = new VotingFacade(gamePlayers, votingFactory);
        var expected      = VotingResult.Empty;

        votingFactory.CreateVoting().Returns(voting);
        voting.GetVotingResult().Returns(expected);
        
        // Act
        sut.StartVote();
        var result = sut.FinishVote();
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Test]
    public void When_FinishVote_And_VotingNotStarted_Then_ExceptionIsThrown()
    {
        // Arrange
        var gamePlayers   = Substitute.For<IGamePlayers>();
        var votingFactory = Substitute.For<IVotingFactory>();
        var sut           = new VotingFacade(gamePlayers, votingFactory);

        // Act
        var act = () => sut.FinishVote();
        
        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}