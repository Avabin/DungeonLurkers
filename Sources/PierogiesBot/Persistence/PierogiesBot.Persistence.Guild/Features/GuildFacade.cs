using PierogiesBot.Persistence.Guild.Features.Many;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Many;

namespace PierogiesBot.Persistence.Guild.Features.Single;

internal class GuildFacade : DocumentFacade<GuildDocument, string, GuildDto>, IGuildFacade
{
    private readonly ISingleGuildService _singleDocumentService;

    public GuildFacade(ISingleGuildService singleDocumentService, IManyGuildsService manyDocumentsService) : base(singleDocumentService, manyDocumentsService)
    {
        _singleDocumentService     = singleDocumentService;
    }

    public async Task AddSubscribedChannelAsync(string id, ulong channelId) =>
        await _singleDocumentService.AddSubscribedChannelAsync(id, channelId);

    public async Task RemoveSubscribedChannelAsync(string id, ulong channelId) => 
        await _singleDocumentService.RemoveSubscribedChannelAsync(id, channelId);

}