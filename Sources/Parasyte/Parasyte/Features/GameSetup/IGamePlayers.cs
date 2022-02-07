using Parasyte.Features.GameSettings;
using Parasyte.Features.Players;

namespace Parasyte.Features.GameSetup;

public interface IGamePlayers
{
    IEnumerable<IPlayer> Players { get; }
    IGameSettings Settings { get; }
    void                   AddPlayer(IPlayer                  player);
    void                   AddPlayers(IEnumerable<IPlayer>    players);
    void                   RemovePlayer(IPlayer               player);
    void                   RemovePlayers(IEnumerable<IPlayer> players);
}