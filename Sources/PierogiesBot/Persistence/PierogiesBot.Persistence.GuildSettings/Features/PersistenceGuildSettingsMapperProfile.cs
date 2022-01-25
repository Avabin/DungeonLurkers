using PierogiesBot.Shared.Features.GuidSettings;
using Shared.Features;

namespace PierogiesBot.Persistence.GuildSettings.Features;

public class PersistenceGuildSettingsMapperProfile
    : DtoMapperProfile<GuildSettingDto, CreateGuildSettingDto, UpdateGuildSettingDto, GuildSettingDocument>
{
}