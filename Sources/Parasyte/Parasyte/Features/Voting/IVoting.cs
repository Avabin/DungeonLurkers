using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

public interface IVoting
{
    IEnumerable<IVote> Votes { get; }

    void AddVote(IVote vote);

    VotingResult GetVotingResult();

    IEnumerable<IGrouping<IPlayer, IVote>> GetDrawTargets();
}