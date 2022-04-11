using System.Net;
using FluentAssertions;
using MongoDB.Bson;
using NUnit.Framework;
using PierogiesBot.Host.Controllers;
using PierogiesBot.Shared.Features.Guilds;
using RestEase;

namespace PierogiesBot.Tests;

[TestFixture]
[Category("Integration")]
[Category("Guild")]
[Category("PierogiesBot")]
[TestOf(typeof(GuildController))]
public class GuildControllerIntegrationTests : GuildIntegrationTestsBase
{
    [Test]
    public async Task GetSubscribedChannels_Success()
    {
        // Arrange
        var doc      = await InsertOne();
        var expected = doc.SubscribedChannels;

        // Act
        var actual = await GuildsApi.GetGuildSubscribedChannelsAsync(doc.Id);

        // Assert
        actual.SubscribedChannels.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task GetSubscribedChannels_WrongId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedChannelsAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSubscribedChannels_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedChannelsAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddChannel_Success()
    {
        // Arrange
        var expected  = await InsertOne();
        var channelId = 872617ul;
        expected.SubscribedChannels.Add(channelId);

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedChannelsAsync(expected.Id, channelId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedChannels.Should().BeEquivalentTo(expected.SubscribedChannels);
    }

    [Test]
    public async Task AddChannel_WrongId()
    {
        // Arrange
        var id        = ":)";
        var channelId = 872617ul;

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedChannelsAsync(id, channelId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AddChannel_NotFound()
    {
        // Arrange
        var id        = ObjectId.GenerateNewId().ToString();
        var channelId = 872617ul;

        // Act
        var act = async () => { await GuildsApi.AddGuildSubscribedChannelsAsync(id, channelId); };

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RemoveChannel_Success()
    {
        // Arrange
        var expected  = await InsertOne();
        var channelId = expected.SubscribedChannels.First();
        expected.SubscribedChannels.Remove(channelId);

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedChannelsAsync(expected.Id, channelId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedChannels.Should().BeEquivalentTo(expected.SubscribedChannels);
    }

    [Test]
    public async Task RemoveChannel_WrongId()
    {
        // Arrange
        var id        = ":)";
        var channelId = 872617ul;

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedChannelsAsync(id, channelId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RemoveChannel_NotFound()
    {
        // Arrange
        var id        = ObjectId.GenerateNewId().ToString();
        var channelId = 872617ul;

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedChannelsAsync(id, channelId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetSubscribedCrontabRules_Success()
    {
        // Arrange
        var doc = await InsertOne();
        var expected = new GuildSubscribedRulesDto
        {
            GuildId  = doc.Id,
            RuleType = RuleType.Scheduled
        };
        expected.SubscribedRules.AddRange(doc.SubscribedCrontabRules);

        // Act
        var actual = await GuildsApi.GetGuildSubscribedCrontabRulesAsync(doc.Id);

        // Assert

        actual.Should().NotBeNull();
        actual.GuildId.Should().Be(expected.GuildId);
        actual.RuleType.Should().Be(expected.RuleType);
        actual.SubscribedRules.Should().BeEquivalentTo(expected.SubscribedRules);
    }

    [Test]
    public async Task GetSubscribedCrontabRules_WrongId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedCrontabRulesAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSubscribedCrontabRules_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedCrontabRulesAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddCrontabRule_Success()
    {
        // Arrange
        var expected = await InsertOne();
        var ruleId   = ObjectId.GenerateNewId().ToString();
        expected.SubscribedCrontabRules.Add(ruleId);

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedCrontabRulesAsync(expected.Id, ruleId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedCrontabRules.Should().BeEquivalentTo(expected.SubscribedCrontabRules);
    }

    [Test]
    public async Task AddCrontabRule_WrongId()
    {
        // Arrange
        var id     = ":)";
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedCrontabRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AddCrontabRule_NotFound()
    {
        // Arrange
        var id     = ObjectId.GenerateNewId().ToString();
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => { await GuildsApi.AddGuildSubscribedCrontabRulesAsync(id, ruleId); };

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RemoveCrontabRule_Success()
    {
        // Arrange
        var expected = await InsertOne();
        var ruleId   = expected.SubscribedCrontabRules.First();
        expected.SubscribedCrontabRules.Remove(ruleId);

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedCrontabRulesAsync(expected.Id, ruleId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedCrontabRules.Should().BeEquivalentTo(expected.SubscribedCrontabRules);
    }

    [Test]
    public async Task RemoveCrontabRule_WrongId()
    {
        // Arrange
        var id     = ":)";
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedCrontabRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RemoveCrontabRule_NotFound()
    {
        // Arrange
        var id     = ObjectId.GenerateNewId().ToString();
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedCrontabRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetSubscribedResponseRules_Success()
    {
        // Arrange
        var doc = await InsertOne();
        var expected = new GuildSubscribedRulesDto
        {
            GuildId  = doc.Id,
            RuleType = RuleType.Response
        };
        expected.SubscribedRules.AddRange(doc.SubscribedResponseRules);

        // Act
        var actual = await GuildsApi.GetGuildSubscribedResponseRulesAsync(doc.Id);

        // Assert

        actual.Should().NotBeNull();
        actual.GuildId.Should().Be(expected.GuildId);
        actual.RuleType.Should().Be(expected.RuleType);
        actual.SubscribedRules.Should().BeEquivalentTo(expected.SubscribedRules);
    }

    [Test]
    public async Task GetSubscribedResponseRules_WrongId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedResponseRulesAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSubscribedResponseRules_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedResponseRulesAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddResponseRule_Success()
    {
        // Arrange
        var expected = await InsertOne();
        var ruleId   = ObjectId.GenerateNewId().ToString();
        expected.SubscribedResponseRules.Add(ruleId);

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedResponseRulesAsync(expected.Id, ruleId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedResponseRules.Should().BeEquivalentTo(expected.SubscribedResponseRules);
    }

    [Test]
    public async Task AddResponseRule_WrongId()
    {
        // Arrange
        var id     = ":)";
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedResponseRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AddResponseRule_NotFound()
    {
        // Arrange
        var id     = ObjectId.GenerateNewId().ToString();
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => { await GuildsApi.AddGuildSubscribedResponseRulesAsync(id, ruleId); };

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RemoveResponseRule_Success()
    {
        // Arrange
        var expected = await InsertOne();
        var ruleId   = expected.SubscribedResponseRules.First();
        expected.SubscribedResponseRules.Remove(ruleId);

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedResponseRulesAsync(expected.Id, ruleId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedResponseRules.Should().BeEquivalentTo(expected.SubscribedResponseRules);
    }

    [Test]
    public async Task RemoveResponseRule_WrongId()
    {
        // Arrange
        var id     = ":)";
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedResponseRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RemoveResponseRule_NotFound()
    {
        // Arrange
        var id     = ObjectId.GenerateNewId().ToString();
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedResponseRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetSubscribedReactionRules_Success()
    {
        // Arrange
        var doc = await InsertOne();
        var expected = new GuildSubscribedRulesDto
        {
            GuildId  = doc.Id,
            RuleType = RuleType.Reaction
        };
        expected.SubscribedRules.AddRange(doc.SubscribedReactionRules);

        // Act
        var actual = await GuildsApi.GetGuildSubscribedReactionRulesAsync(doc.Id);

        // Assert

        actual.Should().NotBeNull();
        actual.GuildId.Should().Be(expected.GuildId);
        actual.RuleType.Should().Be(expected.RuleType);
        actual.SubscribedRules.Should().BeEquivalentTo(expected.SubscribedRules);
    }

    [Test]
    public async Task GetSubscribedReactionRules_WrongId()
    {
        // Arrange
        var id = ":)";

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedReactionRulesAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetSubscribedReactionRules_NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.GetGuildSubscribedReactionRulesAsync(id);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddReactionRule_Success()
    {
        // Arrange
        var expected = await InsertOne();
        var ruleId   = ObjectId.GenerateNewId().ToString();
        expected.SubscribedReactionRules.Add(ruleId);

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedReactionRulesAsync(expected.Id, ruleId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedReactionRules.Should().BeEquivalentTo(expected.SubscribedReactionRules);
    }

    [Test]
    public async Task AddReactionRule_WrongId()
    {
        // Arrange
        var id     = ":)";
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.AddGuildSubscribedReactionRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AddReactionRule_NotFound()
    {
        // Arrange
        var id     = ObjectId.GenerateNewId().ToString();
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => { await GuildsApi.AddGuildSubscribedReactionRulesAsync(id, ruleId); };

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RemoveReactionRule_Success()
    {
        // Arrange
        var expected = await InsertOne();
        var ruleId   = expected.SubscribedReactionRules.First();
        expected.SubscribedReactionRules.Remove(ruleId);

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedReactionRulesAsync(expected.Id, ruleId);

        // Assert
        await act.Should().NotThrowAsync();
        var actual = await GetFromDb(expected.Id);

        actual.Should().NotBeNull();
        actual.SubscribedReactionRules.Should().BeEquivalentTo(expected.SubscribedReactionRules);
    }

    [Test]
    public async Task RemoveReactionRule_WrongId()
    {
        // Arrange
        var id     = ":)";
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedReactionRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RemoveReactionRule_NotFound()
    {
        // Arrange
        var id     = ObjectId.GenerateNewId().ToString();
        var ruleId = ObjectId.GenerateNewId().ToString();

        // Act
        var act = async () => await GuildsApi.RemoveGuildSubscribedReactionRulesAsync(id, ruleId);

        // Assert
        await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
    }
}