using Parasyte.Features.Players;

namespace Parasyte.Features.GameSetup.Exceptions;

public class TooManyRolePlayersException : GameSetupException
{
    public PlayerRole Role { get; }

    public TooManyRolePlayersException(PlayerRole role) : base($"Too many players of role {role:G}")
    {
        Role = role;
    }
}