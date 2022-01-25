using AutoMapper;
using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.GuildSettings.Features.Many;

public class ManyGuildSettingsService
    : ManyDocumentsService<GuildSettingDocument, string, GuildSettingDto>, IManyGuildSettingsService
{
    public ManyGuildSettingsService(IRepository<GuildSettingDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}