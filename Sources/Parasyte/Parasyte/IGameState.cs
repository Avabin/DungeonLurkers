using Parasyte.Features.GameSettings;
using Parasyte.Features.Players;

namespace Parasyte;

public interface IGameState
{
    IEnumerable<IPlayer> Players { get; }
    IGameSettings Settings { get; }
    
    
}