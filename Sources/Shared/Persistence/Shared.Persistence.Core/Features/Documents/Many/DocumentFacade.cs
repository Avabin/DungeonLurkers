using System.Linq.Expressions;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents.Single;

namespace Shared.Persistence.Core.Features.Documents.Many;

public class DocumentFacade<TDocument, TId, TFindDocumentDto>
    : IDocumentFacade<TDocument, TId, TFindDocumentDto>
    where TDocument : IDocument<TId>
    where TFindDocumentDto : IDocumentDto<TId>
{

    public DocumentFacade(
        ISingleDocumentService<TDocument, TId, TFindDocumentDto> singleDocumentService,
        IManyDocumentsService<TDocument, TId, TFindDocumentDto>  manyDocumentsService)
    {
        SingleDocumentService  = singleDocumentService;
        ManyDocumentsService = manyDocumentsService;
    }
    protected ISingleDocumentService<TDocument, TId, TFindDocumentDto> SingleDocumentService  { get; }
    protected IManyDocumentsService<TDocument, TId, TFindDocumentDto>  ManyDocumentsService { get; }

    public Task<TField?> GetFieldAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field) => SingleDocumentService.GetFieldAsync(predicate, field);

    public Task<IEnumerable<TFindDocumentDto>> GetAllAsync(int? skip = null, int? limit = null) =>
        ManyDocumentsService.GetAllAsync(skip, limit);

    public Task<TFindDocumentDto> CreateAsync<TCreateDocDto>(TCreateDocDto request) =>
        SingleDocumentService.CreateAsync(request);

    public Task<TFindDocumentDto?> GetByIdAsync(TId id) => SingleDocumentService.GetByIdAsync(id);

    public Task<TFindDocumentDto?> GetByFieldAsync<TField>(Expression<Func<TDocument, TField>> field, TField value) =>
        SingleDocumentService.GetByFieldAsync(field, value);

    public Task<TFindDocumentDto?> GetByPredicateAsync(Expression<Func<TDocument, bool>> predicate) =>
        SingleDocumentService.GetByPredicateAsync(predicate);

    public Task<IEnumerable<TFindDocumentDto>> GetAllByPredicateAsync(
        Expression<Func<TDocument, bool>> predicate,
        int?                              skip = null,
        int?                              limit = null) => ManyDocumentsService.GetAllByPredicateAsync(predicate, skip, limit);

    public Task UpdateAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field,
        TField                              value) => SingleDocumentService.UpdateAsync(predicate, field, value);

    public Task UpdateAsync<TUpdateDto>(TId id, TUpdateDto dto) => SingleDocumentService.UpdateAsync(id, dto);

    public Task<TFindDocumentDto> DeleteAsync(TId id) => SingleDocumentService.DeleteAsync(id);
}