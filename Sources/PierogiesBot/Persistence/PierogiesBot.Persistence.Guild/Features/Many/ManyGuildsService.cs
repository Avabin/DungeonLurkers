using AutoMapper;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace PierogiesBot.Persistence.Guild.Features.Many;

internal class ManyGuildsService : ManyDocumentsService<GuildDocument,string,GuildDto>, IManyGuildsService
{
    public ManyGuildsService(IRepository<GuildDocument, string> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}