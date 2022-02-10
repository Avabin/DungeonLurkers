﻿using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using PierogiesBot.Host;
using PierogiesBot.Host.Controllers;
using PierogiesBot.Persistence.BotResponseRules.Features;
using PierogiesBot.Shared.Enums;
using PierogiesBot.Shared.Features.BotResponseRules;
using RestEase;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace PierogiesBot.Tests;

[TestFixture]
[Category("Integration")]
[Category("BotResponseRule")]
[Category("PierogiesBot")]
[TestOf(typeof(BotResponseRuleController))]
public class BotResponseRuleControllerIntegrationTests : AuthenticatedTestsBase
{
    [SetUp]
    public async Task SetUp()
    {
        (_rulesClient, Services) =
            await ConfigureResourceServer<Startup, IBotResponseRuleApi>(client => Startup.IdentityHttpClient = client);
    }


    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<BotResponseRuleDocument>(GetServiceFromPierogiesBotHost<IMongoClient>());

        _rulesClient = null;
        Services     = null;
        await base.TearDown();
    }

    public          IServiceProvider   Services     { set; get; } = null!;
    public override UserDocument       TestUser     { get; }
    public override string             ClientId     { get; } = "pierogiesbot";
    public override string             ClientSecret { get; } = "secret";
    public override string             Scope        { get; } = "pierogiesbot.*";
    private         IBotResponseRuleApi _rulesClient = null!;


    public override string Password { get; } = "P4$$w0RD!";

    public BotResponseRuleControllerIntegrationTests()
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
                                   .Select(i => new BotResponseRuleDocument(ResponseMode.First, new []{i.ToString()}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false))
                                              .ToList();
        await GetServiceFromPierogiesBotHost<IMongoRepository<BotResponseRuleDocument>>()
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
        }
    }
    
    [Test]
    public async Task CreateBotResponseRule_Success()
    {
        // Arrange
        var request = new CreateBotResponseRuleDto
        {
            Responses = {"response"},
            TriggerText = "trigger",
            ResponseMode = ResponseMode.First,
            IsTriggerTextRegex = false,
            ShouldTriggerOnContains = false,
            StringComparison = StringComparison.InvariantCultureIgnoreCase
        };
        var mongoRepository = GetServiceFromPierogiesBotHost<IMongoRepository<BotResponseRuleDocument>>();

        // Act
        var result = await _rulesClient.CreateBotResponseRuleAsync(request);
        var saved  = await mongoRepository.GetByIdAsync(result.Id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Responses.Should().BeEquivalentTo(request.Responses);
        result.TriggerText.Should().Be(request.TriggerText);
        result.ResponseMode.Should().Be(request.ResponseMode);
        result.IsTriggerTextRegex.Should().Be(request.IsTriggerTextRegex);
        result.ShouldTriggerOnContains.Should().Be(request.ShouldTriggerOnContains);
        result.StringComparison.Should().Be(request.StringComparison);

        saved.Should().NotBeNull();
        saved.Id.Should().Be(result.Id);
        saved.Responses.Should().BeEquivalentTo(request.Responses);
        saved.TriggerText.Should().Be(request.TriggerText);
        saved.ResponseMode.Should().Be(request.ResponseMode);
        saved.IsTriggerTextRegex.Should().Be(request.IsTriggerTextRegex);
        saved.ShouldTriggerOnContains.Should().Be(request.ShouldTriggerOnContains);
        saved.StringComparison.Should().Be(request.StringComparison);
    }
    
    [Test]
    public async Task GetBotResponseRuleById_Success()
    {
        // Arrange
        var id   = ObjectId.GenerateNewId().ToString();
        var rule = new BotResponseRuleDocument(id, ResponseMode.First, new []{"response"}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false);

        await GetServiceFromPierogiesBotHost<IMongoRepository<BotResponseRuleDocument>>()
           .InsertAsync(rule);

        // Act
        var result = await _rulesClient.FindByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Responses.Should().BeEquivalentTo(rule.Responses);
        result.TriggerText.Should().Be(rule.TriggerText);
        result.ResponseMode.Should().Be(rule.ResponseMode);
        result.IsTriggerTextRegex.Should().Be(rule.IsTriggerTextRegex);
        result.ShouldTriggerOnContains.Should().Be(rule.ShouldTriggerOnContains);
        result.StringComparison.Should().Be(rule.StringComparison);
    }
    
    [Test]
    public async Task RemoveResponseFromRule_Success()
    {
        // Arrange
        var id        = ObjectId.GenerateNewId().ToString();
        var responses = new List<string> {"response1", "response2"};
        var expected  = responses.ToList();
        expected.Remove(responses.First());
        var rule     = new BotResponseRuleDocument(id, ResponseMode.First, responses, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false);

        await GetServiceFromPierogiesBotHost<IMongoRepository<BotResponseRuleDocument>>()
           .InsertAsync(rule);

        // Act
        await _rulesClient.RemoveResponseFromRule(id, responses.First());
        var result = await _rulesClient.FindByIdAsync(id);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Responses.Should().BeEquivalentTo(expected);
        result.TriggerText.Should().Be(rule.TriggerText);
        result.ResponseMode.Should().Be(rule.ResponseMode);
        result.IsTriggerTextRegex.Should().Be(rule.IsTriggerTextRegex);
        result.ShouldTriggerOnContains.Should().Be(rule.ShouldTriggerOnContains);
        result.StringComparison.Should().Be(rule.StringComparison);
    }
    
    [Test]
    public async Task GetBotResponseRuleById_IncorrectId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await _rulesClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GetBotResponseRuleById_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await _rulesClient.FindByIdAsync(id);

        // Assert
        await act.Should().ThrowExactlyAsync<ApiException>().Where(e => e.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task UpdateBotResponseRule_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotResponseRuleDocument(id, ResponseMode.First, new []{"response"}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotResponseRuleDocument>>();
        await repository.InsertAsync(rule);

        var request = new UpdateBotResponseRuleDto
        {
            Responses               = {"🍆"},
            TriggerText             = "trigger12",
            ResponseMode            = ResponseMode.Random,
            IsTriggerTextRegex      = true,
            ShouldTriggerOnContains = true,
            StringComparison        = StringComparison.Ordinal
        };

        // Act
        await _rulesClient.UpdateBotResponseRuleAsync(id, request);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(id);
        saved.Responses.Should().BeEquivalentTo(request.Responses);
        saved.TriggerText.Should().Be(request.TriggerText);
        saved.ResponseMode.Should().Be(request.ResponseMode);
        saved.IsTriggerTextRegex.Should().Be(request.IsTriggerTextRegex);
        saved.ShouldTriggerOnContains.Should().Be(request.ShouldTriggerOnContains);
        saved.StringComparison.Should().Be(request.StringComparison);
    }
    
    [Test]
    public async Task UpdateBotResponseRule_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        var request = new UpdateBotResponseRuleDto
        {
            Responses               = {"🍆"},
            TriggerText             = "trigger12",
            ResponseMode            = ResponseMode.Random,
            IsTriggerTextRegex      = true,
            ShouldTriggerOnContains = true,
            StringComparison        = StringComparison.Ordinal
        };

        // Act
        var act = async () => await _rulesClient.UpdateBotResponseRuleAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task UpdateBotResponseRule_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        var request = new UpdateBotResponseRuleDto
        {
            Responses               = {"🍆"},
            TriggerText             = "trigger12",
            ResponseMode            = ResponseMode.Random,
            IsTriggerTextRegex      = true,
            ShouldTriggerOnContains = true,
            StringComparison        = StringComparison.Ordinal
        };

        // Act
        var act = async () => await _rulesClient.UpdateBotResponseRuleAsync(id, request);

        // Assert

        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeleteBotResponseRule_Success()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();
        var rule = new BotResponseRuleDocument(id, ResponseMode.First, new []{"response"}, "trigger", StringComparison.InvariantCultureIgnoreCase, false, false);

        var repository = GetServiceFromPierogiesBotHost<IMongoRepository<BotResponseRuleDocument>>();
        await repository.InsertAsync(rule);

        // Act
        await _rulesClient.DeleteBotResponseRuleAsync(id);
        var saved = await repository.GetByIdAsync(id);

        // Assert
        saved.Should().BeNull();
    }

    [Test]
    public async Task DeleteBotResponseRule_WrongObjectId()
    {
        // Arrange
        const string id = ":)";

        // Act
        var act = async () => await _rulesClient.DeleteBotResponseRuleAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }
}