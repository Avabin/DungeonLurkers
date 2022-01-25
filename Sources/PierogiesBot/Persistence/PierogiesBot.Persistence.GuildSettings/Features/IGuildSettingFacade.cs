using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.GuildSettings.Features;

public interface IGuildSettingFacade : IDocumentOperationFacade<GuildSettingDocument, string, GuildSettingDto>
{
    Task          SetGuildTimezoneAsync(string tzInfoId, ulong guildId);
    Task<string?> GetGuildTimezoneAsync(ulong  guildId);
    Task          SetMuteRoleAsync(ulong       guildId, ulong roleId);
    Task<ulong>   GetMuteRoleAsync(ulong       guildId);
    Task<GuildSettingDto?>        FindByGuildId(ulong          guildId);
}