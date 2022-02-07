using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

public record Vote(IPlayer Voter, IPlayer Target) : IVote
{
};