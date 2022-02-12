using PierogiesBot.Persistence.GuildSettings.Features.Many;
using PierogiesBot.Persistence.GuildSettings.Features.Single;
using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.GuildSettings.Features;

public class GuildSettingFacade : DocumentOperationFacade<GuildSettingDocument, string, GuildSettingDto>, IGuildSettingFacade
{
    private readonly ISingleGuildSettingService _singleSingleDocumentService;

    public GuildSettingFacade(
        ISingleGuildSettingService singleSingleDocumentService,
        IManyGuildSettingsService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService)
    {
        _singleSingleDocumentService = singleSingleDocumentService;
    }

    public async Task SetGuildTimezoneAsync(string tzInfoId, ulong guildId) => 
        await _singleSingleDocumentService.SetGuildTimezoneAsync(tzInfoId, guildId);

    public async Task<string?> GetGuildTimezoneAsync(ulong guildId) => 
        await _singleSingleDocumentService.GetGuildTimezoneAsync(guildId);

    public async Task SetMuteRoleAsync(ulong guildId, ulong roleId) => 
        await _singleSingleDocumentService.SetMuteRoleAsync(guildId, roleId);

    public async Task<ulong> GetMuteRoleAsync(ulong guildId) => 
        await _singleSingleDocumentService.GetMuteRoleAsync(guildId);

    public async Task<GuildSettingDto?> FindByGuildId(ulong guildId) => 
        await _singleSingleDocumentService.FindByGuildId(guildId); 
}