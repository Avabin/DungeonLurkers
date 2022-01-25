using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using PierogiesBot.Host;
using PierogiesBot.Persistence.BotCrontabRule.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotCrontabRules;
using RestEase;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace PierogiesBot.Tests;

public class BotCrontabRuleControllerIntegrationTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_rulesClient, Services) =
            await ConfigureResourceServer<Startup, IBotCrontabRuleApi>(client => Startup.IdentityHttpClient = client);
    }


    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<BotCrontabRuleDocument>(GetServiceFromPierogiesBotHost<IMongoClient>());

        _rulesClient = null;
        Services     = null;
        await base.TearDown();
    }

    public          IServiceProvider   Services     { set; get; } = null!;
    public override UserDocument       TestUser     { get; }
    public override string             ClientId     { get; } = "pierogiesbot";
    public override string             ClientSecret { get; } = "secret";
    public override string             Scope        { get; } = "pierogiesbot.*";
    private         IBotCrontabRuleApi _rulesClient = null!;


    public override string Password { get; } = "P4$$w0RD!";

    public BotCrontabRuleControllerIntegrationTests()
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
                                   .Select(i => new BotCrontabRuleDocument(false, "* * * *", new []{$"{i}"}, new []{$"{i}"}, ResponseMode.First))
                                   .ToList();
        await GetServiceFromPierogiesBotHost<IMongoRepository<BotCrontabRuleDocument>>()
           .InsertAsync(rules);

        // Act
        var result = await _rulesClient.GetAllAsync(skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = rules.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        foreach (var (doc, dto) in expected.Zip(resultList))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Crontab.Should().Be(dto.Crontab);
            doc.ResponseMode.Should().Be(dto.ResponseMode);
            doc.ReplyEmojis.Should().BeEquivalentTo(dto.ReplyEmojis);
            doc.ReplyMessages.Should().BeEquivalentTo(dto.ReplyMessages);
        }
    }
    
    [Test]
    public async Task CreateBotCrontabRule_Success()
    {
        // Arrange
        var request = new CreateBotCrontabRuleDto
        {
            IsEmoji = false,
            Crontab = "Test BotCrontabRule",
            ResponseMode = ResponseMode.First,
            ReplyEmojis = new []{"Test Emoji"},
            ReplyMessages = new []{"Test Message"},
        };
        var mongoRepository = GetServiceFromPierogiesBotHost<IMongoRepository<BotCrontabRuleDocument>>();

        // Act
        var result = await _rulesClient.CreateBotCrontabRuleAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Crontab.Should().Be(request.Crontab);
        result.ResponseMode.Should().Be(request.ResponseMode);
        result.ReplyEmojis.Should().BeEquivalentTo(request.ReplyEmojis);
        result.ReplyMessages.Should().BeEquivalentTo(request.ReplyMessages);

        saved.Should().NotBeNull();
        saved!.Id.Should().Be(result.Id);
        saved.Crontab.Should().Be(request.Crontab);
        saved.ResponseMode.Should().Be(request.ResponseMode);
        saved.ReplyEmojis.Should().BeEquivalentTo(request.ReplyEmojis);
        saved.ReplyMessages.Should().BeEquivalentTo(request.ReplyMessages);
    }
    
    [Test]
    public async Task GetBotCrontabRuleById_Success()
    {
        // Arrange
        var id   = ObjectId.GenerateNewId().ToString();
        var rule = new BotCrontabRuleDocument(id, false, "* * * *", new []{"Test Emoji"}, new []{"Test Message"}, ResponseMode.First);

        await GetServiceFromPierogiesBotHost<IMongoRepository<BotCrontabRuleDocument>>()
           .InsertAsync(rule);

        // Act
        var result = await _rulesClient.FindByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Crontab.Should().Be(rule.Crontab);
        result.ResponseMode.Should().Be(rule.ResponseMode);
        result.ReplyEmojis.Should().BeEquivalentTo(rule.ReplyEmojis);
        result.ReplyMessages.Should().BeEquivalentTo(rule.ReplyMessages);
    }
    
    [Test]
    public async Task GetBotCrontabRuleById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _rulesClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetBotCrontabRuleById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _rulesClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task UpdateBotCrontabRule_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotCrontabRuleDocument(id, false, "* * * *", new []{"Test Emoji"}, new []{"Test Message"}, ResponseMode.First);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotCrontabRuleDocument>>();
        await repository.InsertAsync(rule);

        var request = new UpdateBotCrontabRuleDto
        {
            IsEmoji = true,
            Crontab = "Test BotCrontabRule",
            ResponseMode = ResponseMode.Random,
            ReplyEmojis = new []{"Test Emoji1"},
            ReplyMessages = new []{"Test Message2"},
        };

        // Act
        await _rulesClient.UpdateBotCrontabRuleAsync(id, request);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(id);
        saved.Crontab.Should().Be(request.Crontab);
        saved.ResponseMode.Should().Be(request.ResponseMode);
        saved.ReplyEmojis.Should().BeEquivalentTo(request.ReplyEmojis);
        saved.ReplyMessages.Should().BeEquivalentTo(request.ReplyMessages);
        saved.IsEmoji.Should().Be(request.IsEmoji);
    }
    
    [Test]
    public async Task UpdateBotCrontabRule_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = new UpdateBotCrontabRuleDto
        {
            IsEmoji = true,
            Crontab = "Test BotCrontabRule",
            ResponseMode = ResponseMode.Random,
            ReplyEmojis = new []{"Test Emoji1"},
            ReplyMessages = new []{"Test Message2"},
        };

        // Act
        var act = async () => await _rulesClient.UpdateBotCrontabRuleAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task UpdateCharacter_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new UpdateBotCrontabRuleDto
        {
            IsEmoji = true,
            Crontab = "Test BotCrontabRule",
            ResponseMode = ResponseMode.Random,
            ReplyEmojis = new []{"Test Emoji1"},
            ReplyMessages = new []{"Test Message2"},
        };

        // Act
        var act = async () => await _rulesClient.UpdateBotCrontabRuleAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeleteBotCrontabRule_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotCrontabRuleDocument(id, false, "* * * *", new []{"Test Emoji"}, new []{"Test Message"}, ResponseMode.First);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotCrontabRuleDocument>>();
        await repository.InsertAsync(rule);

        // Act
        await _rulesClient.DeleteBotCrontabRuleAsync(id);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().BeNull();
    }

    [Test]
    public async Task DeleteBotCrontabRule_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await _rulesClient.DeleteBotCrontabRuleAsync(id);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
}