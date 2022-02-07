using Parasyte.Features.Players;

namespace Parasyte.Features.GameSettings;

public interface IGameSettings
{
    uint MaxPlayerCount   { get; }
    uint MaxEngineerCount { get; }
    uint MaxParasiteCount { get; }
    uint MaxDoctorCount   { get; }
    uint MaxLiarCount     { get; }
    uint MaxGuardianCount { get; }

    uint MaxHumanCount => Math.Max(MaxPlayerCount - (MaxEngineerCount + MaxParasiteCount + MaxDoctorCount + MaxLiarCount + MaxGuardianCount), 0);
    
    public IDictionary<PlayerRole, uint> ToDictionary() => new Dictionary<PlayerRole, uint>
    {
        {PlayerRole.Engineer, MaxEngineerCount},
        {PlayerRole.Parasite, MaxParasiteCount},
        {PlayerRole.Doctor, MaxDoctorCount},
        {PlayerRole.Liar, MaxLiarCount},
        {PlayerRole.Guardian, MaxGuardianCount},
        {PlayerRole.Human, MaxHumanCount}
    };
}