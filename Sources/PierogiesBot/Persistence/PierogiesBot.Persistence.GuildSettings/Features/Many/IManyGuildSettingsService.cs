using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.GuildSettings.Features.Many;

public interface IManyGuildSettingsService : IManyDocumentsService<GuildSettingDocument, string, GuildSettingDto>
{
}