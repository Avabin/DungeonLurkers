using Shared.Features;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Core.Features.Documents.Single;

namespace Shared.Persistence.Mongo.Features.Database.Documents.Single;

public interface IMongoSingleDocumentService<TDocument, TFindDocumentDto>
    : ISingleDocumentService<TDocument, string, TFindDocumentDto>
    where TFindDocumentDto : IDocumentDto<string>
    where TDocument : IDocument<string>
{
}