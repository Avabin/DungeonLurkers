using AutoMapper;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Mongo.Features.Database.Documents.Single;

public class MongoSingleDocumentService<TDocument, TFindDocumentDto>
    : SingleDocumentService<TDocument, string, TFindDocumentDto>,
      IMongoSingleDocumentService<TDocument, TFindDocumentDto>
    where TDocument : IDocument<string>
    where TFindDocumentDto : IDocumentDto<string>
{
    public MongoSingleDocumentService(IRepository<TDocument, string> repository, IMapper mapper) : base(repository,
        mapper)
    {
    }
}