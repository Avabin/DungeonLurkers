using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.GuidSettings;

public interface IGuildSettingApi : IAuthenticatedApi
{
    [Get("GuildSetting")]
    Task<IEnumerable<GuildSettingDto>> GetAllAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("GuildSetting/{id}")] Task<GuildSettingDto> FindByIdAsync([Path] string id);

    [Get("GuildSetting/guild/{guildId}")] Task<GuildSettingDto> FindByGuildIdAsync([Path] ulong guildId);
    [Post("GuildSetting")] Task<GuildSettingDto> CreateGuildSettingAsync([Body] CreateGuildSettingDto createDto);

    [Put("GuildSetting/{id}")] Task UpdateGuildSettingAsync([Path] string id, [Body] UpdateGuildSettingDto updateDto);

    [Delete("GuildSetting/{id}")] Task DeleteGuildSettingAsync([Path] string id);
}