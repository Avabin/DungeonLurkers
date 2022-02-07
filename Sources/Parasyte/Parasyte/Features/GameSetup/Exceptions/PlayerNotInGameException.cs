using Parasyte.Features.Players;

namespace Parasyte.Features.GameSetup.Exceptions;

public class PlayerNotInGameException : Exception
{
    public IPlayer Player { get; }

    public PlayerNotInGameException(IPlayer player) : base($"Player {player.Name} is not in game")
    {
        Player = player;
    }
}