using System.Linq.Expressions;
using AutoMapper;
using Shared.Features;
using Shared.Persistence.Core.Features.Exceptions;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Core.Features.Documents.Single;

public class SingleDocumentService<TDocument, TId, TFindDocumentDto>
    : DocumentServiceBase<TDocument, TId>,
      ISingleDocumentService<TDocument, TId, TFindDocumentDto>
    where TFindDocumentDto : IDocumentDto<TId>
    where TDocument : IDocument<TId>
{
    public SingleDocumentService(IRepository<TDocument, TId> repository, IMapper mapper) : base(repository, mapper)
    {
    }
    public async Task<TField?> GetFieldAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field)
    {
        var fieldValue = await Repository.GetFieldAsync(predicate, field);

        if (fieldValue is null) throw new DocumentNotFoundException("Nothing found!");

        return fieldValue;
    }

    public async Task<TFindDocumentDto> CreateAsync<TCreateDocDto>(TCreateDocDto request)
    {
        var doc = Mapper.Map<TDocument>(request);
        ThrowIfNull(doc);
        await Repository.InsertAsync(doc);
        return Mapper.Map<TFindDocumentDto>(doc);
    }

    public async Task<TFindDocumentDto?> GetByIdAsync(TId id)
    {
        var doc = await Repository.GetByIdAsync(id);

        return doc is not null ? Mapper.Map<TFindDocumentDto>(doc) : default;
    }

    public async Task<TFindDocumentDto?> GetByFieldAsync<TField>(Expression<Func<TDocument, TField>> field, TField value)
    {
        var result = await Repository.GetByFieldAsync(field, value);

        return result is null ? default : Mapper.Map<TFindDocumentDto>(result);
    }

    public async Task<TFindDocumentDto?> GetByPredicateAsync(Expression<Func<TDocument, bool>> predicate)
    {
        var result = await Repository.GetByPredicateAsync(predicate);

        return result is null ? default : Mapper.Map<TFindDocumentDto>(result);
    }

    public Task UpdateAsync<TField>(
        Expression<Func<TDocument, bool>>   predicate,
        Expression<Func<TDocument, TField>> field,
        TField                              value) => Repository.UpdateAllAsync(predicate, field, value);

    public Task UpdateAsync<TUpdateDto>(TId id, TUpdateDto dto)
    {
        var doc = Mapper.Map<TDocument>(dto);

        return Repository.UpdateAsync(id, doc);
    }

    public async Task<TFindDocumentDto> DeleteAsync(TId id)
    {
        var existing = await Repository.GetByIdAsync(id);

        if (existing is null) throw new DocumentNotFoundException($"Document with ID = {id} not found!");

        await Repository.DeleteAsync(existing.Id);
        return Mapper.Map<TFindDocumentDto>(existing);
    }

    protected static void ThrowIfNull(TDocument? doc)
    {
        if (doc is null) throw new ArgumentException("Invalid request!");
    }
}