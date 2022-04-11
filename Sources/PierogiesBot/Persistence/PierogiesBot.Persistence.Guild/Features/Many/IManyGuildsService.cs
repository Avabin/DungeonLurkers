using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.Guild.Features.Many;

public interface IManyGuildsService : IManyDocumentsService<GuildDocument, string, GuildDto>
{
}