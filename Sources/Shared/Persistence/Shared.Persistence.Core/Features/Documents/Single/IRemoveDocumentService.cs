using Shared.Features;

namespace Shared.Persistence.Core.Features.Documents.Single;

public interface IRemoveDocumentService<TDocument, in TId, TFindDocumentDto>
    where TDocument : IDocument<TId> where TFindDocumentDto : IDocumentDto<TId>
{
    Task<TFindDocumentDto> DeleteAsync(TId id);
}