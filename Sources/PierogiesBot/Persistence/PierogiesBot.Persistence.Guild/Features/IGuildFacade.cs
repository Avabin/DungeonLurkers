using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.Guild.Features;

public interface IGuildFacade : IDocumentFacade<GuildDocument, string, GuildDto>
{
    Task AddSubscribedChannelAsync(string id, ulong channelId);
    Task RemoveSubscribedChannelAsync(string   id, ulong channelId);
}