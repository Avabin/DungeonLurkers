namespace Parasyte.Features.GameSettings;

public record GameSettings(uint MaxPlayerCount = 10, uint MaxEngineerCount = 2, uint MaxParasiteCount = 2,
                           uint MaxDoctorCount = 1, uint MaxLiarCount = 1, uint MaxGuardianCount = 1) : IGameSettings;