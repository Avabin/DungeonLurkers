using System;
using System.Collections.Generic;
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
using TheDungeonGuide.Persistence.Sessions;
using TheDungeonGuide.Sessions.Host;
using TheDungeonGuide.Sessions.Host.Controllers;
using TheDungeonGuide.Shared.Features.Sessions;
#pragma warning disable CS1591

namespace TheDungeonGuide.Sessions.Tests;

[TestFixture]
[Category(nameof(Sessions))]
[Category("Integration")]
[Category("TheDungeonGuide")]
[TestOf(typeof(SessionController))]
[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
public class SessionControllerTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_sessionsClient, Services) =
            await ConfigureResourceServer<Startup, ISessionsApi>(client => Startup.IdentityHttpClient = client);
    }

    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<SessionDocument>(GetServiceFromSessionsCube<IMongoClient>());
        await base.TearDown();
    }

    public IServiceProvider Services { get; set; } = null!;

    public override UserDocument TestUser { get; }

    public override string       Password     { get; } = @"P4$$\/\/0Rd!";
    public override string       ClientId     { get; } = "sessions";
    public override string       ClientSecret { get; } = "secret";
    public override string       Scope        { get; } = "sessions.*";
    private         ISessionsApi _sessionsClient = null!;

    public SessionControllerTests()
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
        var sessions = Enumerable.Range(0, limit ?? 10)
                                 .Select(i => new SessionDocument($"Session #{i}", TestUser.Id))
                                 .ToList();
        await GetServiceFromSessionsCube<IMongoRepository<SessionDocument>>()
           .InsertAsync(sessions);

        // Act
        var result = await _sessionsClient.GetSessionsAsync(skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = sessions.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        foreach (var (doc, dto) in expected.Zip(resultList))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Title.Should().Be(dto.Title);
            doc.GameMasterId.Should().Be(dto.GameMasterId);
        }
    }

    [Test]
    public async Task CreateSession_Success()
    {
        // Arrange
        var request = new CreateSessionDto
        {
            Title        = "Test session",
            GameMasterId = TestUser.Id,
        };
        var mongoRepository = GetServiceFromSessionsCube<IMongoRepository<SessionDocument>>();

        // Act
        var result = await _sessionsClient.CreateSessionAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be(request.Title);
        result.GameMasterId.Should().Be(request.GameMasterId);
        result.PlayersIds.Should().BeEquivalentTo(request.PlayersIds);
        result.CharactersIds.Should().BeEquivalentTo(request.CharactersIds);

        saved.Should().NotBeNull();
        saved!.Id.Should().Be(result.Id);
        saved.Title.Should().Be(request.Title);
        saved.GameMasterId.Should().Be(request.GameMasterId);
        saved.PlayersIds.Should().BeEquivalentTo(request.PlayersIds);
        saved.CharactersIds.Should().BeEquivalentTo(request.CharactersIds);
    }

    [Test]
    public async Task CreateSession_IncorrectGameMasterId()
    {
        // Arrange
        var request = new CreateSessionDto
        {
            Title        = "Test session",
            GameMasterId = ":)",
        };

        // Act
        var act = async () => await _sessionsClient.CreateSessionAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSessionById_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var session = new SessionDocument
        {
            Id           = id,
            Title        = "Test session",
            GameMasterId = TestUser.Id,
        };

        await GetServiceFromSessionsCube<IMongoRepository<SessionDocument>>()
           .InsertAsync(session);

        // Act
        var result = await _sessionsClient.GetSessionByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be(session.Title);
        result.GameMasterId.Should().Be(session.GameMasterId);
    }

    [Test]
    public async Task GetSessionById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _sessionsClient.GetSessionByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSessionById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _sessionsClient.GetSessionByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetSessionsByGameMasterId_Success()
    {
        // Arrange
        var ids      = Enumerable.Range(0, 10).Select(_ => ObjectId.GenerateNewId().ToString());
        var sessions = ids.Select(x => new SessionDocument($"TestSession {x}", TestUser.Id, x)).ToList();

        await CreateDocuments(sessions, GetServiceFromSessionsCube<IMongoClient>());

        // Act
        var result = await _sessionsClient.GetSessionsByGameMasterIdAsync(TestUser.Id);

        // Assert
        foreach (var (doc, dto) in sessions.Zip(result))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Title.Should().Be(dto.Title);
            doc.GameMasterId.Should().Be(dto.GameMasterId);
        }
    }

    [Test]
    public async Task GetSessionsByCharacterId_Success()
    {
        // Arrange
        var ids         = Enumerable.Range(0, 10).Select(_ => ObjectId.GenerateNewId().ToString());
        var characterId = ObjectId.GenerateNewId().ToString();
        var sessions = ids.Select(x => new SessionDocument($"TestSession {x}", TestUser.Id, x)
        {
            CharactersIds =
            {
                characterId,
            },
        }).ToList();

        await CreateDocuments(sessions, GetServiceFromSessionsCube<IMongoClient>());

        // Act
        var result = await _sessionsClient.GetSessionsByCharacterIdAsync(characterId);

        // Assert
        foreach (var (doc, dto) in sessions.Zip(result))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Title.Should().Be(dto.Title);
            doc.GameMasterId.Should().Be(dto.GameMasterId);
        }
    }

    [Test]
    public async Task RemovePlayerFromSession_Success()
    {
        // Arrange
        var ids      = Enumerable.Range(0, 10).Select(_ => ObjectId.GenerateNewId().ToString());
        var playerId = ObjectId.GenerateNewId().ToString();
        var session = new SessionDocument
        {
            Title        = "Test session",
            GameMasterId = TestUser.Id,
            PlayersIds =
            {
                playerId,
            },
            CharactersIds = ids.ToList(),
        };
        var expected = session with
        {
            PlayersIds = new List<string>(),
        };
        await CreateDocument(session, GetServiceFromSessionsCube<IMongoClient>());

        // Act
        await _sessionsClient.RemovePlayerFromSessionAsync(session.Id, playerId);
        var actual = await GetServiceFromSessionsCube<IMongoRepository<SessionDocument>>().GetByIdAsync(session.Id);

        // Assert
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(expected.Id);
        actual.Title.Should().Be(expected.Title);
        actual.GameMasterId.Should().Be(expected.GameMasterId);
        actual.PlayersIds.Should().BeEmpty();
        actual.CharactersIds.Should().BeEquivalentTo(expected.CharactersIds);
    }

    [Test]
    public async Task RemoveCharacterFromSession_Success()
    {
        // Arrange
        var ids         = Enumerable.Range(0, 10).Select(_ => ObjectId.GenerateNewId().ToString());
        var characterId = ObjectId.GenerateNewId().ToString();
        var session = new SessionDocument
        {
            Title        = "Test session",
            GameMasterId = TestUser.Id,
            PlayersIds   = ids.ToList(),
            CharactersIds =
            {
                characterId,
            },
        };
        var expected = session with
        {
            CharactersIds = new List<string>(),
        };
        await CreateDocument(session, GetServiceFromSessionsCube<IMongoClient>());

        // Act
        await _sessionsClient.RemoveCharacterFromSessionAsync(session.Id, characterId);
        var actual = await GetServiceFromSessionsCube<IMongoRepository<SessionDocument>>().GetByIdAsync(session.Id);

        // Assert
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(expected.Id);
        actual.Title.Should().Be(expected.Title);
        actual.GameMasterId.Should().Be(expected.GameMasterId);
        actual.CharactersIds.Should().BeEmpty();
        actual.PlayersIds.Should().BeEquivalentTo(expected.PlayersIds);
    }

    private T GetServiceFromSessionsCube<T>() where T : notnull => Services.GetRequiredService<T>();
}