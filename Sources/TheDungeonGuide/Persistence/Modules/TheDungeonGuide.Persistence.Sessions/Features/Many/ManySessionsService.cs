using AutoMapper;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;
using TheDungeonGuide.Shared.Features.Sessions;

namespace TheDungeonGuide.Persistence.Sessions.Features.Many;

public class ManySessionsService : ManyDocumentsService<SessionDocument, string, SessionDto>, IManySessionsService
{
    public ManySessionsService(IRepository<SessionDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }

    public async Task<IEnumerable<SessionDto>> GetAllByMemberIdAsync(string id, int? skip = null, int? limit = null)
    {
        var found = await Repository.GetAllByPredicateAsync(x => x.PlayersIds.Contains(id) || x.GameMasterId == id, skip, limit);
        
        var mapped = Mapper.Map<IEnumerable<SessionDto>>(found);
        return mapped;
    }
}