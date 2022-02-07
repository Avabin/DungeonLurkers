using System.Collections.Concurrent;
using System.Collections.Immutable;
using Parasyte.Features.GameSettings;
using Parasyte.Features.GameSetup.Exceptions;
using Parasyte.Features.Players;

namespace Parasyte.Features.GameSetup;

internal class GamePlayers : IGamePlayers
{
    private readonly Random                        _random  = Random.Shared;
    private readonly List<IPlayer>                 _players = new(10);
    private readonly IDictionary<PlayerRole, uint> _rolesCount;
    private readonly IDictionary<PlayerRole, uint> _maxRolesCount;
    public           IEnumerable<IPlayer>          Players  => _players.ToImmutableList();
    public           IGameSettings                 Settings { get; }

    public GamePlayers(IGameSettings settings)
    {
        Settings          = settings;
        _players.Capacity = (int) Settings.MaxPlayerCount;

        var maxRoles = Settings.ToDictionary().ToList();
        _maxRolesCount = new ConcurrentDictionary<PlayerRole, uint>(maxRoles);
        _rolesCount    = new ConcurrentDictionary<PlayerRole, uint>(maxRoles.Select(x => new KeyValuePair<PlayerRole, uint>(x.Key, 0u)));
    }

    public void AddPlayer(IPlayer player)
    {
        if (_players.Count >= Settings.MaxPlayerCount)
            throw new TooManyPlayersException(Settings.MaxPlayerCount);
        
        var playerRole             = player.PlayerRole;
        var playerRoleCurrentCount = _rolesCount[playerRole];
        var playerRoleMaxCount = _maxRolesCount[playerRole];
        
        if (playerRoleCurrentCount >= playerRoleMaxCount)
            throw new TooManyRolePlayersException(playerRole);
        
        _players.Add(player);
        _rolesCount[playerRole] = playerRoleCurrentCount + 1;
    }

    public void AddPlayers(IEnumerable<IPlayer> players)
    {
        foreach (var player in players)
        {
            AddPlayer(player);
        }
    }

    public void RemovePlayer(IPlayer player)
    {
        if (!_players.Contains(player))
            throw new PlayerNotInGameException(player);
        
        var playerRole             = player.PlayerRole;
        var playerRoleCurrentCount = _rolesCount[playerRole];
        _players.Remove(player);
        _rolesCount[playerRole] = playerRoleCurrentCount - 1;
    }

    public void RemovePlayers(IEnumerable<IPlayer> players)
    {
        foreach (var player in players)
        {
            RemovePlayer(player);
        }
    }
}