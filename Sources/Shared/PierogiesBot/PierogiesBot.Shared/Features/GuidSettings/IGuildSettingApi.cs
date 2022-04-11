using RestEase;
using Shared.Features.Authentication;

namespace PierogiesBot.Shared.Features.GuidSettings;

public interface IGuildSettingApi : IAuthenticatedApi
{
    [Get("{PathPrefix}/GuildSetting")]
    Task<IEnumerable<GuildSettingDto>> GetAllAsync([Query] int? skip = null, [Query] int? limit = null);

    [Get("{PathPrefix}/GuildSetting/{id}")] Task<GuildSettingDto> FindByIdAsync([Path] string id);

    [Get("{PathPrefix}/GuildSetting/guild/{guildId}")] Task<GuildSettingDto> FindByGuildIdAsync([Path] ulong guildId);
    [Post("{PathPrefix}/GuildSetting")] Task<GuildSettingDto> CreateGuildSettingAsync([Body] CreateGuildSettingDto createDto);

    [Put("{PathPrefix}/GuildSetting/{id}")] Task UpdateGuildSettingAsync([Path] string id, [Body] UpdateGuildSettingDto updateDto);

    [Delete("{PathPrefix}/GuildSetting/{id}")] Task DeleteGuildSettingAsync([Path] string id);
}