namespace Parasyte.Features.GameSetup.Exceptions;

public class TooManyPlayersException : GameSetupException
{
    public uint MaxPlayers { get; }

    public TooManyPlayersException(uint maxPlayers) : base($"Too many players! Max players {maxPlayers}")
    {
        MaxPlayers = maxPlayers;
    }
}