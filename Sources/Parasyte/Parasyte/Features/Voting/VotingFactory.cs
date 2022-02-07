namespace Parasyte.Features.Voting;

internal class VotingFactory : IVotingFactory
{
    public IVoting CreateVoting() => new Voting();
}