using AutoMapper;
using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.GuildSettings.Features.Single;

public class SingleGuildSettingService
    : SingleDocumentService<GuildSettingDocument, string, GuildSettingDto>, ISingleGuildSettingService
{
    public SingleGuildSettingService(IRepository<GuildSettingDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }

    public async Task SetGuildTimezoneAsync(string tzInfoId, ulong guildId) => 
        await Repository.UpdateSingleAsync(x => x.GuildId == guildId, x => x.GuildTimeZone, tzInfoId);

    public async Task<string?> GetGuildTimezoneAsync(ulong guildId) => 
        await Repository.GetFieldAsync(x => x.GuildId == guildId, x => x.GuildTimeZone);

    public async Task SetMuteRoleAsync(ulong guildId, ulong roleId) => 
        await Repository.UpdateSingleAsync(x => x.GuildId == guildId, x => x.GuildMuteRoleId, roleId);

    public async Task<ulong> GetMuteRoleAsync(ulong guildId) => 
        await Repository.GetFieldAsync(x => x.GuildId == guildId, x => x.GuildMuteRoleId);

    public async Task<GuildSettingDto?> FindByGuildId(ulong guildId)
    {
        var doc =  await Repository.GetByFieldAsync(x => x.GuildId, guildId);
        
        return doc is null ? null : Mapper.Map<GuildSettingDto>(doc); 
    }
}