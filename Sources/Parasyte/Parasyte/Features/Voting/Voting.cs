using System.Collections.Immutable;
using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

internal class Voting : IVoting
{
    private readonly List<IVote>        _votes = new();
    public           IEnumerable<IVote> Votes => _votes.ToImmutableList();

    public void AddVote(IVote vote) => _votes.Add(vote);

    public VotingResult GetVotingResult()
    {
        if (!_votes.Any()) return VotingResult.Empty;
        var winners = GetDrawTargets().ToArray();
        var isDraw  = winners.Length > 1 && winners.First().Count() == winners.Last().Count();
        return isDraw
                   ? VotingResult.Draw(winners.First().Key, winners.Last().Key)
                   : VotingResult.Win(winners.First().Key);
    }

    public IEnumerable<IGrouping<IPlayer, IVote>> GetDrawTargets()
    {
        return _votes
              .GroupBy(x => x.Target)
              .OrderByDescending(x => x.Count())
              .Take(2);
    }
}