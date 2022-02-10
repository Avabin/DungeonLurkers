using AutoMapper;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Mongo.Features.Database.Documents.Many;

public class MongoManyDocumentsService<TDocument, TFindDocumentDto>
    : ManyDocumentsService<TDocument, string, TFindDocumentDto>, IMongoManyDocumentsService<TDocument, TFindDocumentDto>
    where TDocument : class, IDocument<string> where TFindDocumentDto : IDocumentDto<string>
{
    public MongoManyDocumentsService(IRepository<TDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}