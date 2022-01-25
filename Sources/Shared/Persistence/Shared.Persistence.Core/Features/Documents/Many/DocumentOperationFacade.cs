using System.Linq.Expressions;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents.Single;

namespace Shared.Persistence.Core.Features.Documents.Many;

public class DocumentOperationFacade<TDocument, TId, TFindDocumentDto>
    : IDocumentOperationFacade<TDocument, TId, TFindDocumentDto>
    where TDocument : IDocument<TId>
    where TFindDocumentDto : IDocumentDto<TId>
{

    public DocumentOperationFacade(
        ISingleDocumentService<TDocument, TId, TFindDocumentDto> singleDocumentService,
        IManyDocumentsService<TDocument, TId, TFindDocumentDto>  manyDocumentsService)
    {
        DocumentService  = singleDocumentService;
        DocumentsService = manyDocumentsService;
    }
    protected ISingleDocumentService<TDocument, TId, TFindDocumentDto> DocumentService  { get; }
    protected IManyDocumentsService<TDocument, TId, TFindDocumentDto>  DocumentsService { get; }

    public Task<TField?> GetFieldAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field) => DocumentService.GetFieldAsync(predicate, field);

    public Task<IEnumerable<TFindDocumentDto>> GetAllAsync(int? skip = null, int? limit = null) =>
        DocumentsService.GetAllAsync(skip, limit);

    public Task<TFindDocumentDto> CreateAsync<TCreateDocDto>(TCreateDocDto request) =>
        DocumentService.CreateAsync(request);

    public Task<TFindDocumentDto?> GetByIdAsync(TId id) => DocumentService.GetByIdAsync(id);

    public Task<TFindDocumentDto?> GetByFieldAsync<TField>(Expression<Func<TDocument, TField>> field, TField value) =>
        DocumentService.GetByFieldAsync(field, value);

    public Task<TFindDocumentDto?> GetByPredicateAsync(Expression<Func<TDocument, bool>> predicate) =>
        DocumentService.GetByPredicateAsync(predicate);

    public Task<IEnumerable<TFindDocumentDto>> GetAllByPredicateAsync(
        Expression<Func<TDocument, bool>> predicate,
        int?                              skip = null,
        int?                              limit = null) => DocumentsService.GetAllByPredicateAsync(predicate, skip, limit);

    public Task UpdateAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field,
        TField                              value) => DocumentService.UpdateAsync(predicate, field, value);

    public Task UpdateAsync<TUpdateDto>(TId id, TUpdateDto dto) => DocumentService.UpdateAsync(id, dto);

    public Task<TFindDocumentDto> DeleteAsync(TId id) => DocumentService.DeleteAsync(id);
}