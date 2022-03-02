using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using NUnit.Framework;
using RestEase;
using Shared.Features;

namespace Tests.Shared;

public abstract class CrudIntegrationTestBase<TDocument, TDto, TCreateDto, TUpdateDto> : AuthenticatedTestsBase
    where TDocument : IDocument<string>
    where TDto : IDocumentDto<string>
{
    protected abstract Func<int?, int?, Task<IEnumerable<TDto>>> SutGetAllFunc { get; }
    protected abstract Func<TCreateDto, Task<TDto>>              SutCreateFunc { get; }

    protected abstract Func<string, Task<TDto>>       SutGetByIdFunc { get; }
    protected abstract Func<string, TUpdateDto, Task> SutUpdateFunc  { get; }

    protected abstract Func<string, Task> SutDeleteFunc { get; }
    protected abstract Task<IEnumerable<TDocument>> InsertMany(int count);
    protected abstract void VerifyMany(IEnumerable<TDocument> documents, IEnumerable<TDto> dtos);

    [TestCase(null, null)]
    [TestCase(0,    1)]
    [TestCase(5,    10)]
    public async Task GetAll_Paginated(int? skip, int? limit)
    {
        // Arrange
        var docs = await InsertMany(limit ?? 10);

        // Act
        var result = await SutGetAllFunc(skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = docs.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        VerifyMany(expected, resultList);
    }

    protected abstract Task<TDocument> InsertOne();
    protected abstract TCreateDto      GetCreateDto();
    protected abstract void            VerifySingle(TDocument document, TDto       dto);
    protected abstract void            VerifySingle(TDocument document, TCreateDto dto);

    [Test]
    public async Task CreateDocument_Success()
    {
        // Arrange
        var request = GetCreateDto();

        // Act
        var result = await SutCreateFunc(request);
        var saved  = await GetFromDb(result.Id);

        // Assert
        VerifySingle(saved, request);
        VerifySingle(saved, result);
    }

    [Test]
    public async Task GetDocumentById_Success()
    {
        // Arrange
        var saved = await InsertOne();
        var id    = saved.Id;

        // Act
        var result = await SutGetByIdFunc(id);

        // Assert
        VerifySingle(saved, result);
    }

    [Test]
    public async Task GetDocumentById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await SutGetByIdFunc(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetDocumentById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await SutGetByIdFunc(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }

    protected abstract Task<TDocument?> GetFromDb(string id);
    protected abstract TUpdateDto       GetUpdateDto();
    protected abstract void             VerifySingle(TDocument? document, TUpdateDto dto);

    [Test]
    public async Task UpdateDocument_Success()
    {
        // Arrange
        var doc = await InsertOne();
        var id  = doc.Id;

        var request = GetUpdateDto();

        // Act
        await SutUpdateFunc(id, request);
        var saved = await GetFromDb(id);

        // Assert
        VerifySingle(saved, request);
    }

    [Test]
    public async Task UpdateDocument_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = GetUpdateDto();

        // Act
        var act = async () => await SutUpdateFunc(id, request);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDocument_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = GetUpdateDto();

        // Act
        var act = async () => await SutUpdateFunc(id, request);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteDocument_Success()
    {
        // Arrange
        var doc = await InsertOne();
        var id  = doc.Id;

        // Act
        await SutDeleteFunc(id);
        var saved = await GetFromDb(id);

        // Assert
        saved.Should().BeNull();
    }


    [Test]
    public async Task DeleteDocument_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await SutDeleteFunc(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteDocument_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await SutDeleteFunc(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
}