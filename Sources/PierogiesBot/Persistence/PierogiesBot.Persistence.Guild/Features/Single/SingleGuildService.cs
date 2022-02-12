using AutoMapper;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.Guild.Features.Single;

internal class SingleGuildService : SingleDocumentService<GuildDocument, string, GuildDto>, ISingleGuildService
{
    public SingleGuildService(IRepository<GuildDocument, string> repository, IMapper mapper) : base(repository, mapper)
    {
    }

    public async Task AddSubscribedChannelAsync(string id, ulong channelId) => 
        await Repository.AddElementToArrayFieldAsync(id, x => x.SubscribedChannels, channelId);

    public async Task RemoveSubscribedChannelAsync(string id, ulong channelId) => 
        await Repository.RemoveElementFromArrayFieldAsync(id, x => x.SubscribedChannels, channelId);
}