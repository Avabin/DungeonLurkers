using Parasyte.Features.GameSetup;
using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

public interface IVotingFacade
{
    IVoting?     CurrentVoting { get; }
    IGamePlayers Voters       { get; }
    void         StartVote();
    void         AddVote(IPlayer voter, IPlayer target);
    VotingResult FinishVote();
}