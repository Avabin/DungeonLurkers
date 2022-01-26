using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using PierogiesBot.Host;
using PierogiesBot.Host.Controllers;
using PierogiesBot.Persistence.GuildSettings.Features;
using PierogiesBot.Shared.Features.GuidSettings;
using RestEase;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace PierogiesBot.Tests;

[TestFixture]
[Category("Integration")]
[Category("GuildSetting")]
[TestOf(typeof(GuildSettingController))]
public class GuildSettingControllerIntegrationTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_settingsClient, Services) =
            await ConfigureResourceServer<Startup, IGuildSettingApi>(client => Startup.IdentityHttpClient = client);
    }


    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<GuildSettingDocument>(GetServiceFromPierogiesBotHost<IMongoClient>());

        _settingsClient = null;
        Services        = null;
        await base.TearDown();
    }

    public          IServiceProvider Services     { set; get; } = null!;
    public override UserDocument     TestUser     { get; }
    public override string           ClientId     { get; } = "pierogiesbot";
    public override string           ClientSecret { get; } = "secret";
    public override string           Scope        { get; } = "pierogiesbot.*";
    private         IGuildSettingApi _settingsClient = null!;


    public override string Password { get; } = "P4$$w0RD!";

    public GuildSettingControllerIntegrationTests()
    {
        TestUser = new()
        {
            Id       = ObjectId.GenerateNewId().ToString(),
            UserName = $"pb_test_user_{TestUserSuffix:N}",
            Email    = "testuser@pierogiesbot.ocm",
            Roles =
            {
                "user",
                "admin"
            },
        };
    }

    private T GetServiceFromPierogiesBotHost<T>() where T : notnull => Services.GetRequiredService<T>();

    [TestCase(null, null)]
    [TestCase(0,    1)]
    [TestCase(5,    10)]
    public async Task GetAll_Paginated(int? skip, int? limit)
    {
        // Arrange
        var rules = Enumerable.Range(0, limit ?? 10)
                              .Select(i => new GuildSettingDocument(182387961728379ul, "Europe/Warsaw", 1231151531ul))
                              .ToList();
        await GetServiceFromPierogiesBotHost<IMongoRepository<GuildSettingDocument>>()
           .InsertAsync(rules);

        // Act
        var result = await _settingsClient.GetAllAsync(skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = rules.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        foreach (var (doc, dto) in expected.Zip(resultList))
        {
            doc.Id.Should().Be(dto.Id);
        }
    }

    [Test]
    public async Task CreateGuildSetting_Success()
    {
        // Arrange
        var request = new CreateGuildSettingDto
        {
            GuildId = 182387961728379ul,
            GuildTimeZone = "Europe/Warsaw",
            GuildMuteRoleId = 1231151531ul
        };
        var mongoRepository = GetServiceFromPierogiesBotHost<IMongoRepository<GuildSettingDocument>>();

        // Act
        var result = await _settingsClient.CreateGuildSettingAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.GuildId.Should().Be(request.GuildId);
        result.GuildTimeZone.Should().Be(request.GuildTimeZone);
        result.GuildMuteRoleId.Should().Be(request.GuildMuteRoleId);

        saved.Should().NotBeNull();
        saved.Id.Should().Be(result.Id);
        saved.GuildId.Should().Be(request.GuildId);
        saved.GuildTimeZone.Should().Be(request.GuildTimeZone);
        saved.GuildMuteRoleId.Should().Be(request.GuildMuteRoleId);
    }

    [Test]
    public async Task GetGuildSettingById_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new GuildSettingDocument(id, 182387961728379ul, "Europe/Warsaw", 1231151531ul);

        await GetServiceFromPierogiesBotHost<IMongoRepository<GuildSettingDocument>>()
           .InsertAsync(rule);

        // Act
        var result = await _settingsClient.FindByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Id.Should().Be(id);
        result.GuildId.Should().Be(rule.GuildId);
        result.GuildTimeZone.Should().Be(rule.GuildTimeZone);
        result.GuildMuteRoleId.Should().Be(rule.GuildMuteRoleId);
    }

    [Test]
    public async Task GetGuildSettingById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _settingsClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetGuildSettingById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _settingsClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task GetGuildSettingByGuildId_Success()
    {
        // Arrange
        var id      = ObjectId.GenerateNewId().ToString();
        var guildId = 182387961728379ul;
        var rule    = new GuildSettingDocument(id, guildId, "Europe/Warsaw", 1231151531ul);

        await GetServiceFromPierogiesBotHost<IMongoRepository<GuildSettingDocument>>()
           .InsertAsync(rule);

        // Act
        var result = await _settingsClient.FindByGuildIdAsync(guildId);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Id.Should().Be(id);
        result.GuildId.Should().Be(rule.GuildId);
        result.GuildTimeZone.Should().Be(rule.GuildTimeZone);
        result.GuildMuteRoleId.Should().Be(rule.GuildMuteRoleId);
    }
    
    [Test]
    public async Task GetGuildSettingByGuildId_NotFound()
    {
        // Arrange
        var guildId = 182387961728379ul;

        // Act
        var act = async () => await _settingsClient.FindByGuildIdAsync(guildId);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateGuildSetting_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new GuildSettingDocument(id, 182387961728379ul, "Europe/Warsaw", 1231151531ul);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<GuildSettingDocument>>();
        await repository.InsertAsync(rule);

        var request = new UpdateGuildSettingDto
        {
            GuildId = 182387961728379ul,
            GuildTimeZone = "Europe/Warsaw",
            GuildMuteRoleId = 1231151531ul
        };

        // Act
        await _settingsClient.UpdateGuildSettingAsync(id, request);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(id);
        saved.GuildId.Should().Be(request.GuildId);
        saved.GuildTimeZone.Should().Be(request.GuildTimeZone);
        saved.GuildMuteRoleId.Should().Be(request.GuildMuteRoleId);
    }

    [Test]
    public async Task UpdateGuildSetting_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = new UpdateGuildSettingDto
        {
            GuildId = 182387961728379ul,
            GuildTimeZone = "Europe/Warsaw",
            GuildMuteRoleId = 1231151531ul
        };

        // Act
        var act = async () => await _settingsClient.UpdateGuildSettingAsync(id, request);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateGuildSetting_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new UpdateGuildSettingDto
        {
            GuildId = 182387961728379ul,
            GuildTimeZone = "Europe/Warsaw",
            GuildMuteRoleId = 1231151531ul
        };

        // Act
        var act = async () => await _settingsClient.UpdateGuildSettingAsync(id, request);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteGuildSetting_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new GuildSettingDocument(id, 182387961728379ul, "Europe/Warsaw", 1231151531ul);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<GuildSettingDocument>>();
        await repository.InsertAsync(rule);

        // Act
        await _settingsClient.DeleteGuildSettingAsync(id);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().BeNull();
    }
    
    

    [Test]
    public async Task DeleteGuildSetting_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await _settingsClient.DeleteGuildSettingAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task DeleteGuildSetting_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _settingsClient.DeleteGuildSettingAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
}