using Shared.Features;
using Shared.MessageBroker.Core;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Mongo.Features.Database.Documents.Many;
using Shared.Persistence.Mongo.Features.Database.Documents.Single;

namespace Shared.Persistence.Mongo.Features.Database.Documents;

public class MongoDocumentOperationFacade<TDocument, TFindDocumentDto>
    : DocumentOperationFacade<TDocument, string, TFindDocumentDto>,
      IMongoDocumentOperationFacade<TDocument, TFindDocumentDto>
      where TFindDocumentDto : IDocumentDto<string>
    where TDocument : IDocument<string>
{

    public MongoDocumentOperationFacade(
        IMongoSingleDocumentService<TDocument, TFindDocumentDto> singleDocumentService,
        IMongoManyDocumentsService<TDocument, TFindDocumentDto>  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService)
    {
    }
}