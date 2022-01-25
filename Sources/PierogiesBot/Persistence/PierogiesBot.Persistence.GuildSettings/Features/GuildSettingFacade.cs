using PierogiesBot.Persistence.GuildSettings.Features.Many;
using PierogiesBot.Persistence.GuildSettings.Features.Single;
using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.GuildSettings.Features;

public class GuildSettingFacade : DocumentOperationFacade<GuildSettingDocument, string, GuildSettingDto>, IGuildSettingFacade
{
    private readonly ISingleGuildSettingService _singleDocumentService;

    public GuildSettingFacade(
        ISingleGuildSettingService singleDocumentService,
        IManyGuildSettingsService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService = singleDocumentService;
    }

    public async Task SetGuildTimezoneAsync(string tzInfoId, ulong guildId) => 
        await _singleDocumentService.SetGuildTimezoneAsync(tzInfoId, guildId);

    public async Task<string?> GetGuildTimezoneAsync(ulong guildId) => 
        await _singleDocumentService.GetGuildTimezoneAsync(guildId);

    public async Task SetMuteRoleAsync(ulong guildId, ulong roleId) => 
        await _singleDocumentService.SetMuteRoleAsync(guildId, roleId);

    public async Task<ulong> GetMuteRoleAsync(ulong guildId) => 
        await _singleDocumentService.GetMuteRoleAsync(guildId);

    public async Task<GuildSettingDto?> FindByGuildId(ulong guildId) => 
        await _singleDocumentService.FindByGuildId(guildId); 
}