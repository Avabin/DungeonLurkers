namespace Parasyte.Features.Players;

public interface IPlayer
{
    string      Name  { get; }
    PlayerRole        PlayerRole  { get; }
    PlayerState State { get; }
}