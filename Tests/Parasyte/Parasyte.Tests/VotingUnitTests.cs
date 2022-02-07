using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Parasyte.Features.Players;
using Parasyte.Features.Voting;

namespace Parasyte.Tests;

[TestFixture]
[TestOf(typeof(Voting))]
[Category(nameof(Parasyte))]
[Category("Unit")]
public class VotingUnitTests
{
    [Test]
    public void When_AddVote_Then_VoteCountIncreases_And_WinnerIsNotNull()
    {
        // Arrange
        var voter  = Substitute.For<IPlayer>();
        var target = Substitute.For<IPlayer>();
        var vote   = new Vote(voter, target);

        var sut = new Voting();
        
        // Act
        sut.AddVote(vote);
        var result = sut.GetVotingResult();
        
        // Assert
        sut.Votes.Should().HaveCount(1, "vote was added");
        result.Winner.Should().NotBeNull("vote was added so there is a winner");
        result.IsDraw.Should().BeFalse("there is odd number of votes");
        result.DrawWinner.Should().BeNull("there is odd number of votes");
    }
    
    [Test]
    public void When_EvenNumberOfVotes_Then_IsDraw_And_ThereAreTwoWinners()
    {
        // Arrange
        var voter  = Substitute.For<IPlayer>();
        var target1 = Substitute.For<IPlayer>();
        var target2 = Substitute.For<IPlayer>();
        var voteFor1   = new Vote(voter, target1);
        var voteFor2   = new Vote(voter, target2);

        var sut = new Voting();
        
        // Act
        sut.AddVote(voteFor1);
        sut.AddVote(voteFor2);
        var result = sut.GetVotingResult();
        
        // Assert
        sut.Votes.Should().HaveCount(2, "two votes were added");
        result.Winner.Should().NotBeNull("there is even number of votes so a draw");
        result.IsDraw.Should().BeTrue("there is even number of votes so a draw");
        result.DrawWinner.Should().NotBeNull("there is even number of votes so a draw");
    }
    
    [Test]
    public void When_NoVote_Then_WinnerIsNull()
    {
        // Arrange
        var sut = new Voting();
        
        // Act
        var result = sut.GetVotingResult();
        
        // Assert
        sut.Votes.Should().HaveCount(0, "no vote was added");
        result.Winner.Should().BeNull("no vote was added");
    }
}