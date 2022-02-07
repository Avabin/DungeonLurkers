using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

public interface IVote
{
    IPlayer Voter { get; }
    IPlayer Target { get; }
}