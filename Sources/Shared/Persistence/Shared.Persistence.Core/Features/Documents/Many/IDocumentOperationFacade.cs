using Shared.Features;
using Shared.Persistence.Core.Features.Documents.Single;

namespace Shared.Persistence.Core.Features.Documents.Many;

public interface IDocumentOperationFacade<TDocument, in TId, TFindDocumentDto>
    : ISingleDocumentService<TDocument, TId, TFindDocumentDto>,
      IManyDocumentsService<TDocument, TId, TFindDocumentDto>
    where TDocument : IDocument<TId>
    where TFindDocumentDto : IDocumentDto<TId>
{
}