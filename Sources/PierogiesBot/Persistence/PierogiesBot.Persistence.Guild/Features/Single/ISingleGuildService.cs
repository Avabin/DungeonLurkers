using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Single;

namespace PierogiesBot.Persistence.Guild.Features.Single;

public interface ISingleGuildService : ISingleDocumentService<GuildDocument, string, GuildDto>
{
    Task AddSubscribedChannelAsync(string id, ulong channelId);
    Task RemoveSubscribedChannelAsync(string id, ulong channelId);
}