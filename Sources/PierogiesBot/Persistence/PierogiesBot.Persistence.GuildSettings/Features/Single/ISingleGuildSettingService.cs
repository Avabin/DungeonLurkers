using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.GuildSettings.Features.Single;

public interface ISingleGuildSettingService : ISingleDocumentService<GuildSettingDocument, string, GuildSettingDto>
{
    Task          SetGuildTimezoneAsync(string tzInfoId, ulong guildId);
    Task<string?> GetGuildTimezoneAsync(ulong  guildId);
    Task          SetMuteRoleAsync(ulong       guildId, ulong roleId);
    Task<ulong>   GetMuteRoleAsync(ulong       guildId);
    Task<GuildSettingDto?>        FindByGuildId(ulong          guildId);
}