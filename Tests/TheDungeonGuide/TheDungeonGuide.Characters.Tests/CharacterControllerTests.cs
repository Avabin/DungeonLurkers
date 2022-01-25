using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using RestEase;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;
using TheDungeonGuide.Characters.Host;
using TheDungeonGuide.Persistence.Characters;
using TheDungeonGuide.Shared.Features.Characters;

namespace TheDungeonGuide.Characters.Tests;

[TestFixture]
[Category(nameof(Characters))]
[Category("Integration")]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
public class CharacterControllerTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_charactersClient, Services) =
            await ConfigureResourceServer<Startup, ICharactersApi>(client => Startup.UsersHttpClient = client);
    }

    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<CharacterDocument>(GetServiceFromCharactersCube<IMongoClient>());

        _charactersClient = null;
        Services          = null;
        await base.TearDown();
    }
    public IServiceProvider Services { get; set; } = null!;

    public override UserDocument TestUser { get; }

    public override string         Password     { get; } = @"P4$$\/\/0Rd!";
    public override string         ClientId     { get; } = "characters";
    public override string         ClientSecret { get; } = "secret";
    public override string         Scope        { get; } = "characters.*";
    private         ICharactersApi _charactersClient = null!;
    public CharacterControllerTests()
    {
        TestUser = new()
        {
            Id       = ObjectId.GenerateNewId().ToString(),
            UserName = $"test_user_{TestUserSuffix:N}",
            Email    = "testuser@abhgsjkd.ocm",
            Roles =
            {
                "user",
                "admin",
                "gm",
            },
        };
    }

    [TestCase(null, null)]
    [TestCase(0,    1)]
    [TestCase(5,    10)]
    public async Task GetAll_Paginated(int? skip, int? limit)
    {
        // Arrange
        var characters = Enumerable.Range(0, limit ?? 10)
                                   .Select(i => new CharacterDocument(TestUser.Id, name: $"Character #{i}"))
                                   .ToList();
        await GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>()
           .InsertAsync(characters);

        // Act
        var result = await _charactersClient.GetAllAsync(skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = characters.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        foreach (var (doc, dto) in expected.Zip(resultList))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Name.Should().Be(dto.Name);
        }
    }

    [TestCase(null, null)]
    [TestCase(0,    1)]
    [TestCase(5,    10)]
    public async Task FindByOwnerId_Paginated(int? skip, int? limit)
    {
        // Arrange
        var characters = Enumerable.Range(0, limit ?? 10)
                                   .Select(i => new CharacterDocument(TestUser.Id, name: $"Character #{i}"))
                                   .ToList();
        await GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>()
           .InsertAsync(characters);

        // Act
        var result = await _charactersClient.FindByOwnerIdAsync(TestUser.Id, skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = characters.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        foreach (var (doc, dto) in expected.Zip(resultList))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Name.Should().Be(dto.Name);
        }
    }

    [Test]
    public async Task CreateCharacter_Success()
    {
        // Arrange
        var request = new CreateCharacterDto
        {
            Name = "Test character",
        };
        var mongoRepository = GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>();

        // Act
        var result = await _charactersClient.CreateCharacterAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(request.Name);

        saved.Should().NotBeNull();
        saved!.Id.Should().Be(result.Id);
        saved.Name.Should().Be(request.Name);
    }

    [Test]
    public async Task GetCharacterById_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var character = new CharacterDocument
        {
            Id   = id,
            Name = "Test character",
        };

        await GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>()
           .InsertAsync(character);

        // Act
        var result = await _charactersClient.FindByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(character.Name);
    }

    [Test]
    public async Task GetCharacterById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _charactersClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetCharacterById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _charactersClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetCharacterByName_Success()
    {
        // Arrange
        var name = "Test character";
        var expected = new CharacterDocument
        {
            OwnerId = TestUser.Id,
            Name    = name,
        };
        await GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>()
           .InsertAsync(expected);

        // Act
        var actual = await _charactersClient.FindCharacterByNameAsync(name);

        // Assert
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        actual.OwnerId.Should().Be(expected.OwnerId);
    }

    [Test]
    public async Task GetCharacterByName_NotFound()
    {
        // Arrange
        var name = "name";

        // Act
        var act = async () => await _charactersClient.FindCharacterByNameAsync(name);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateCharacter_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var character = new CharacterDocument
        {
            Id      = id,
            Name    = "Test character",
            OwnerId = TestUser.Id,
        };

        var repository = GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>();
        await repository.InsertAsync(character);

        var request = new UpdateCharacterDto
        {
            Name    = "NewName",
            OwnerId = TestUser.Id,
        };

        // Act
        await _charactersClient.UpdateCharacterAsync(id, request);
        var saved = await repository.GetByIdAsync(id);

        // Assert

        saved.Should().NotBeNull();
        saved!.Id.Should().Be(id);
        saved.Name.Should().Be(request.Name);
        saved.OwnerId.Should().Be(character.OwnerId);
    }
    [Test]
    public async Task UpdateCharacter_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = new UpdateCharacterDto
        {
            Name    = "NewName",
            OwnerId = TestUser.Id,
        };

        // Act
        var act = async () => await _charactersClient.UpdateCharacterAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateCharacter_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new UpdateCharacterDto
        {
            Name    = "NewName",
            OwnerId = TestUser.Id,
        };

        // Act
        var act = async () => await _charactersClient.UpdateCharacterAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteCharacter_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var character = new CharacterDocument
        {
            Id      = id,
            Name    = "Test character",
            OwnerId = TestUser.Id,
        };

        var repository = GetServiceFromCharactersCube<IMongoRepository<CharacterDocument>>();
        await repository.InsertAsync(character);

        // Act
        await _charactersClient.DeleteCharacterAsync(id);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().BeNull();
    }

    [Test]
    public async Task DeleteCharacter_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await _charactersClient.DeleteCharacterAsync(id);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }


    private T GetServiceFromCharactersCube<T>() where T : notnull => Services.GetRequiredService<T>();
}