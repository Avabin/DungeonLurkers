using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using PierogiesBot.Host;
using PierogiesBot.Host.Controllers;
using PierogiesBot.Persistence.BotReactRules.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotReactRules;
using RestEase;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace PierogiesBot.Tests;

[TestFixture]
[Category("Integration")]
[Category("BotReactRule")]
[Category("PierogiesBot")]
[TestOf(typeof(BotReactRuleController))]
public class BotReactRuleControllerIntegrationTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_rulesClient, Services) =
            await ConfigureResourceServer<Startup, IBotReactionRuleApi>(client => Startup.IdentityHttpClient = client);
    }


    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<BotReactionRuleDocument>(GetServiceFromPierogiesBotHost<IMongoClient>());

        _rulesClient = null;
        Services     = null;
        await base.TearDown();
    }

    public          IServiceProvider   Services     { set; get; } = null!;
    public override UserDocument       TestUser     { get; }
    public override string             ClientId     { get; } = "pierogiesbot";
    public override string             ClientSecret { get; } = "secret";
    public override string             Scope        { get; } = "pierogiesbot.*";
    private         IBotReactionRuleApi _rulesClient = null!;


    public override string Password { get; } = "P4$$w0RD!";

    public BotReactRuleControllerIntegrationTests()
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
                                   .Select(i => new BotReactionRuleDocument(new []{""}, $"trigger{i}", StringComparison.InvariantCultureIgnoreCase, false, false, ResponseMode.First))
                                   .ToList();
        await GetServiceFromPierogiesBotHost<IMongoRepository<BotReactionRuleDocument>>()
           .InsertAsync(rules);

        // Act
        var result = await _rulesClient.GetAllReactionRulesAsync(skip, limit);

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
    public async Task CreateBotReactRule_Success()
    {
        // Arrange
        var request = new CreateBotReactionRuleDto
        {
            Reactions = {"🍆"},
            TriggerText = "trigger",
            ResponseMode = ResponseMode.First,
            IsTriggerTextRegex = false,
            ShouldTriggerOnContains = false,
            StringComparison = StringComparison.InvariantCultureIgnoreCase
        };
        var mongoRepository = GetServiceFromPierogiesBotHost<IMongoRepository<BotReactionRuleDocument>>();

        // Act
        var result = await _rulesClient.CreateBotReactionRuleAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Reactions.Should().BeEquivalentTo(request.Reactions);
        result.TriggerText.Should().Be(request.TriggerText);
        result.ResponseMode.Should().Be(request.ResponseMode);
        result.IsTriggerTextRegex.Should().Be(request.IsTriggerTextRegex);
        result.ShouldTriggerOnContains.Should().Be(request.ShouldTriggerOnContains);
        result.StringComparison.Should().Be(request.StringComparison);

        saved.Should().NotBeNull();
        saved.Id.Should().Be(result.Id);
        saved.Reactions.Should().BeEquivalentTo(request.Reactions);
        saved.TriggerText.Should().Be(request.TriggerText);
        saved.ResponseMode.Should().Be(request.ResponseMode);
        saved.IsTriggerTextRegex.Should().Be(request.IsTriggerTextRegex);
        saved.ShouldTriggerOnContains.Should().Be(request.ShouldTriggerOnContains);
        saved.StringComparison.Should().Be(request.StringComparison);
    }
    
    [Test]
    public async Task GetBotReactRuleById_Success()
    {
        // Arrange
        var id   = ObjectId.GenerateNewId().ToString();
        var rule = new BotReactionRuleDocument(id, new []{"reaction"}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false, ResponseMode.First);

        await GetServiceFromPierogiesBotHost<IMongoRepository<BotReactionRuleDocument>>()
           .InsertAsync(rule);

        // Act
        var result = await _rulesClient.FindReactionRuleByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Reactions.Should().BeEquivalentTo(rule.Reactions);
        result.TriggerText.Should().Be(rule.TriggerText);
        result.ResponseMode.Should().Be(rule.ResponseMode);
        result.IsTriggerTextRegex.Should().Be(rule.IsTriggerTextRegex);
        result.ShouldTriggerOnContains.Should().Be(rule.ShouldTriggerOnContains);
        result.StringComparison.Should().Be(rule.StringComparison);
    }
    
    [Test]
    public async Task GetBotReactRuleById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _rulesClient.FindReactionRuleByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetBotReactRuleById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _rulesClient.FindReactionRuleByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task UpdateBotReactRule_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotReactionRuleDocument(id, new []{"reaction"}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false, ResponseMode.First);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotReactionRuleDocument>>();
        await repository.InsertAsync(rule);

        var request = new UpdateBotReactionRuleDto
        {
            Reactions = {"🍆"},
            TriggerText = "trigger12",
            ResponseMode = ResponseMode.Random,
            IsTriggerTextRegex = true,
            ShouldTriggerOnContains = true,
            StringComparison = StringComparison.Ordinal
        };

        // Act
        await _rulesClient.UpdateBotReactRuleAsync(id, request);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(id);
        saved.Reactions.Should().BeEquivalentTo(request.Reactions);
        saved.TriggerText.Should().Be(request.TriggerText);
        saved.ResponseMode.Should().Be(request.ResponseMode);
        saved.IsTriggerTextRegex.Should().Be(request.IsTriggerTextRegex);
        saved.ShouldTriggerOnContains.Should().Be(request.ShouldTriggerOnContains);
        saved.StringComparison.Should().Be(request.StringComparison);
    }
    
    [Test]
    public async Task UpdateBotReactRule_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = new UpdateBotReactionRuleDto
        {
            Reactions = {"🍆"},
            TriggerText = "trigger12",
            ResponseMode = ResponseMode.Random,
            IsTriggerTextRegex = true,
            ShouldTriggerOnContains = true,
            StringComparison = StringComparison.Ordinal
        };

        // Act
        var act = async () => await _rulesClient.UpdateBotReactRuleAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task UpdateBotReactRule_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new UpdateBotReactionRuleDto
        {
            Reactions = {"🍆"},
            TriggerText = "trigger12",
            ResponseMode = ResponseMode.Random,
            IsTriggerTextRegex = true,
            ShouldTriggerOnContains = true,
            StringComparison = StringComparison.Ordinal
        };

        // Act
        var act = async () => await _rulesClient.UpdateBotReactRuleAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeleteBotReactRule_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotReactionRuleDocument(id, new []{"reaction"}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false, ResponseMode.First);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotReactionRuleDocument>>();
        await repository.InsertAsync(rule);

        // Act
        await _rulesClient.DeleteBotReactionRuleAsync(id);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().BeNull();
    }

    [Test]
    public async Task DeleteBotReactRule_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await _rulesClient.DeleteBotReactionRuleAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
}