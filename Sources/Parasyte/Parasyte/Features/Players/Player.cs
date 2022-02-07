namespace Parasyte.Features.Players;

internal class Player : IPlayer
{
    public Player(string name, PlayerRole playerRole, PlayerState state)
    {
        Name       = name;
        PlayerRole = playerRole;
        State      = state;
    }

    public string      Name       { get; init; }
    public PlayerRole  PlayerRole { get; private set; }
    public PlayerState State      { get; init; }
}