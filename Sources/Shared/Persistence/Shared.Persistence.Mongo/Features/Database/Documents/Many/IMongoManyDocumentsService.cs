using Shared.Features;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Many;

namespace Shared.Persistence.Mongo.Features.Database.Documents.Many;

public interface IMongoManyDocumentsService<TDocument, TFindDocumentDto>
    : IManyDocumentsService<TDocument, string, TFindDocumentDto>
    where TFindDocumentDto : IDocumentDto<string>
    where TDocument : IDocument<string>
{
}