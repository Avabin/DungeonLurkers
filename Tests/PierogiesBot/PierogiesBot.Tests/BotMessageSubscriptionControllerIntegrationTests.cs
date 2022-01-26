using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using PierogiesBot.Host;
using PierogiesBot.Host.Controllers;
using PierogiesBot.Persistence.BotMessageSubscription.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotMessageSubscriptions;
using RestEase;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace PierogiesBot.Tests;

[TestFixture]
[Category("Integration")]
[Category("BotMessageSubscription")]
[TestOf(typeof(BotMessageSubscriptionController))]
public class BotMessageSubscriptionControllerIntegrationTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_subscriptionApi, Services) =
            await ConfigureResourceServer<Startup, IBotMessageSubscriptionApi>(client => Startup.IdentityHttpClient = client);
    }


    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<BotMessageSubscriptionDocument>(GetServiceFromPierogiesBotHost<IMongoClient>());

        _subscriptionApi = null;
        Services     = null;
        await base.TearDown();
    }

    public          IServiceProvider   Services     { set; get; } = null!;
    public override UserDocument       TestUser     { get; }
    public override string             ClientId     { get; } = "pierogiesbot";
    public override string             ClientSecret { get; } = "secret";
    public override string             Scope        { get; } = "pierogiesbot.*";
    private         IBotMessageSubscriptionApi _subscriptionApi = null!;


    public override string Password { get; } = "P4$$w0RD!";

    public BotMessageSubscriptionControllerIntegrationTests()
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
                                   .Select(i => new BotMessageSubscriptionDocument(123123123ul + (ulong)i, 8761872368123ul + (ulong)i, SubscriptionType.Responses))
                                   .ToList();
        await GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>()
           .InsertAsync(rules);

        // Act
        var result = await _subscriptionApi.GetAllAsync(skip, limit);

        // Assert
        var resultList = result.ToList();
        var expected   = rules.Skip(skip ?? 0).ToList();
        resultList.Count.Should().Be(expected.Count);

        foreach (var (doc, dto) in expected.Zip(resultList))
        {
            doc.Id.Should().Be(dto.Id);
            doc.ChannelId.Should().Be(dto.ChannelId);
            doc.GuildId.Should().Be(dto.GuildId);
            doc.SubscriptionType.Should().Be(dto.SubscriptionType);

        }
    }

    [Test]
    public async Task GetAllForChannel_Success()
    {
        // Arrange
        var guildId   = 9867871628736ul;
        var channelId = 123123123ul;
        var rules = Enumerable.Range(0, 10)
                                   .Select(i => new BotMessageSubscriptionDocument(guildId, 8761872368123ul + (ulong)i, SubscriptionType.Responses))
                                    .Select((x, i) => i % 2 == 0 ? x with {ChannelId = channelId} : x)
                                   .ToList();

        var channelRulesCount = rules.Count(x => x.ChannelId == channelId);
        
        await GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>()
           .InsertAsync(rules);
        
        // Act
        var result = await _subscriptionApi.FindAllForChannelAsync(guildId, channelId);
        
        // Assert
        result.Count().Should().Be(channelRulesCount);
    }
    
    [Test]
    public async Task GetAllForGuild_Success()
    {
        // Arrange
        var guildId   = 9867871628736ul;
        var rules = Enumerable.Range(0, 10)
                              .Select(i => new BotMessageSubscriptionDocument(guildId, 8761872368123ul + (ulong)i, SubscriptionType.Responses))
                              .Select((x, i) => i % 2 == 0 ? x with {GuildId = guildId} : x)
                              .ToList();

        var channelRulesCount = rules.Count(x => x.GuildId == guildId);
        
        await GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>()
           .InsertAsync(rules);
        
        // Act
        var result = await _subscriptionApi.FindAllForGuildAsync(guildId);
        
        // Assert
        result.Count().Should().Be(channelRulesCount);
    }
    
    [Test]
    public async Task CreateBotMessageSubscription_Success()
    {
        // Arrange
        var request = new CreateBotMessageSubscriptionDto
        {
            ChannelId = 12312312312ul,
            GuildId   = 8761872368123ul,
            SubscriptionType = SubscriptionType.Responses
        };
        var mongoRepository = GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>();

        // Act
        var result = await _subscriptionApi.CreateBotMessageSubscriptionAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();

        saved.Should().NotBeNull();
        saved!.Id.Should().Be(result.Id);
        saved!.ChannelId.Should().Be(request.ChannelId);
        saved!.GuildId.Should().Be(request.GuildId);
        saved!.SubscriptionType.Should().Be(request.SubscriptionType);
    }
    
    [Test]
    public async Task GetBotMessageSubscriptionById_Success()
    {
        // Arrange
        var id   = ObjectId.GenerateNewId().ToString();
        var rule = new BotMessageSubscriptionDocument(id,123123123ul, 123123123ul, SubscriptionType.Responses);

        await GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>()
           .InsertAsync(rule);

        // Act
        var result = await _subscriptionApi.FindByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Id.Should().Be(id);
        result.ChannelId.Should().Be(rule.ChannelId);
        result.GuildId.Should().Be(rule.GuildId);
        result.SubscriptionType.Should().Be(rule.SubscriptionType);
    }
    
    [Test]
    public async Task GetBotMessageSubscriptionById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _subscriptionApi.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetBotMessageSubscriptionById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _subscriptionApi.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task UpdateBotMessageSubscription_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotMessageSubscriptionDocument(id,123123123ul, 123123123ul, SubscriptionType.Responses);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>();
        await repository.InsertAsync(rule);

        var request = new UpdateBotMessageSubscriptionDto
        {
            ChannelId = 12312312312ul,
            GuildId   = 8761872368123ul,
            SubscriptionType = SubscriptionType.Responses
        };

        // Act
        await _subscriptionApi.UpdateBotMessageSubscriptionAsync(id, request);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(id);
        saved!.ChannelId.Should().Be(request.ChannelId);
        saved!.GuildId.Should().Be(request.GuildId);
        saved!.SubscriptionType.Should().Be(request.SubscriptionType);
    }
    
    [Test]
    public async Task UpdateBotMessageSubscription_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = new UpdateBotMessageSubscriptionDto
        {
            ChannelId = 123123551UL,
            GuildId   = 8761872368123ul,
            SubscriptionType = SubscriptionType.Responses
        };

        // Act
        var act = async () => await _subscriptionApi.UpdateBotMessageSubscriptionAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task UpdateCharacter_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new UpdateBotMessageSubscriptionDto
        {
            ChannelId = 871628561823ul,
            GuildId   = 8761872368123ul,
            SubscriptionType = SubscriptionType.Responses
        };

        // Act
        var act = async () => await _subscriptionApi.UpdateBotMessageSubscriptionAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeleteBotMessageSubscription_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotMessageSubscriptionDocument(id,123123123ul, 123123123ul, SubscriptionType.Responses);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotMessageSubscriptionDocument>>();
        await repository.InsertAsync(rule);

        // Act
        await _subscriptionApi.DeleteBotMessageSubscriptionAsync(id);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().BeNull();
    }

    [Test]
    public async Task DeleteBotMessageSubscription_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await _subscriptionApi.DeleteBotMessageSubscriptionAsync(id);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
}