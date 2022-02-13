using Parasyte.Features.GameSetup;
using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

public class VotingFacade : IVotingFacade
{
    private readonly IVotingFactory _votingFactory;
    public           IVoting?       CurrentVoting { get; private set; }
    public           IGamePlayers   Voters       { get; }

    public VotingFacade(IGamePlayers voters, IVotingFactory votingFactory)
    {
        _votingFactory = votingFactory;
        Voters             = voters;
    }
    
    public void StartVote() => CurrentVoting =_votingFactory.CreateVoting();

    public void AddVote(IPlayer voter, IPlayer target)
    {
        if(CurrentVoting is null)
            throw new InvalidOperationException("Voting is not started");

        if (Voters.Players.Contains(voter) && Voters.Players.Contains(target))
        {
            CurrentVoting.AddVote(new Vote(voter, target));
        }
        else
        {
            throw new ArgumentException("Player is not in voters");
        }
    }

    public VotingResult FinishVote()
    {
        if(CurrentVoting is null)
            throw new InvalidOperationException("Voting is not started");

        var result = CurrentVoting.GetVotingResult();
        CurrentVoting = null;
        
        return result;
    }
}