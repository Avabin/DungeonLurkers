using Parasyte.Features.Players;

namespace Parasyte.Features.Voting;

public record VotingResult(bool IsDraw, IPlayer? Winner, IPlayer? DrawWinner)
{
    public static VotingResult Empty => new(false, null, null);
    public static VotingResult Draw(IPlayer first, IPlayer second)                =>new(true, first, second);
    public static VotingResult Win(IPlayer player) => new(false, player, null);
};