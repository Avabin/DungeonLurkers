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
}