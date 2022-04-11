using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using PierogiesBot.Host;
using PierogiesBot.Persistence.Guild;
using PierogiesBot.Shared.Features.Guilds;
using Shared.Infrastructure;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace PierogiesBot.Tests;

public abstract class
    GuildIntegrationTestsBase : CrudIntegrationTestBase<GuildDocument, GuildDto, CreateGuildDto, UpdateGuildDto>
{
    protected GuildIntegrationTestsBase()
    {
        TestUser = new UserDocument
        {
            Id       = ObjectId.GenerateNewId().ToString(),
            UserName = $"pb_test_user_{TestUserSuffix:N}",
            Email    = "testuser@pierogiesbot.ocm",
            Roles =
            {
                "user",
                "admin"
            }
        };
    }

    protected       IGuildsApi       GuildsApi    { get; set; } = null!;
    public          IServiceProvider Services     { set; get; } = null!;
    public override string           ClientId     { get; }      = "pierogiesbot";
    public override string           ClientSecret { get; }      = "secret";
    public override UserDocument     TestUser     { get; }
    public override string           Password     { get; } = "P4$$w0RD!";
    public override string           Scope        { get; } = "pierogiesbot.*";

    protected override Func<int?, int?, Task<IEnumerable<GuildDto>>> SutGetAllFunc => GuildsApi.GetGuildsAsync;

    protected override Func<CreateGuildDto, Task<GuildDto>> SutCreateFunc => GuildsApi.CreateGuildAsync;

    protected override Func<string, Task<GuildDto>> SutGetByIdFunc => GuildsApi.GetGuildAsync;

    protected override Func<string, UpdateGuildDto, Task> SutUpdateFunc => GuildsApi.UpdateGuildAsync;

    protected override Func<string, Task> SutDeleteFunc => GuildsApi.DeleteGuildAsync;

    [SetUp]
    public async Task SetUp()
    {
        (GuildsApi, Services) =
            await ConfigureResourceServer<Startup, IGuildsApi>(client => StartupBase.IdentityHttpClient = client);
    }


    [TearDown]
    public override async Task TearDown()
    {
        await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

        await ClearCollection<GuildDocument>(GetServiceFromPierogiesBotHost<IMongoClient>());

        GuildsApi = null;
        Services  = null;
        await base.TearDown();
    }

    protected T GetServiceFromPierogiesBotHost<T>() where T : notnull
    {
        return Services.GetRequiredService<T>();
    }

    protected override async Task<IEnumerable<GuildDocument>> InsertMany(int count)
    {
        var guilds = Enumerable.Range(0, count)
                               .Select(i => new GuildDocument($"Guild {i}", "", 12312312312ul, "", new List<ulong>(),
                                                              new List<string>(), new List<string>(),
                                                              new List<string>()))
                               .ToList();
        await GetServiceFromPierogiesBotHost<IMongoRepository<GuildDocument>>()
           .InsertAsync(guilds);

        return guilds;
    }

    protected override void VerifyMany(IEnumerable<GuildDocument> documents, IEnumerable<GuildDto> dtos)
    {
        foreach (var (doc, dto) in documents.Zip(dtos))
        {
            doc.Id.Should().Be(dto.Id);
            doc.Name.Should().Be(dto.Name);
            doc.DiscordId.Should().Be(dto.DiscordId);
            doc.TimezoneId.Should().Be(dto.TimezoneId);
            doc.IconUri.Should().Be(dto.IconUri);
            doc.SubscribedChannels.Should().BeEquivalentTo(dto.SubscribedChannels);
            doc.SubscribedReactionRules.Should().BeEquivalentTo(dto.SubscribedReactionRules);
            doc.SubscribedResponseRules.Should().BeEquivalentTo(dto.SubscribedResponseRules);
        }
    }

    protected override async Task<GuildDocument> InsertOne()
    {
        var doc = new GuildDocument("Guild", "", 12312312312ul, "", new List<ulong>
                                    {
                                        12312312312ul,
                                        12312312313ul,
                                        12312312314ul
                                    }, new List<string>
                                    {
                                        "scheduled-message-1",
                                        "scheduled-message-2",
                                        "scheduled-message-3",
                                        "scheduled-message-4"
                                    },
                                    new List<string>
                                    {
                                        "response-message-1",
                                        "response-message-2",
                                        "response-message-3",
                                        "response-message-4"
                                    }, new List<string>
                                    {
                                        "reaction-1",
                                        "reaction-2",
                                        "reaction-3",
                                        "reaction-4"
                                    });
        var mongoRepository = GetServiceFromPierogiesBotHost<IMongoRepository<GuildDocument>>();
        await mongoRepository.InsertAsync(doc);

        return doc;
    }

    protected override CreateGuildDto GetCreateDto()
    {
        var createDto = new CreateGuildDto
        {
            DiscordId               = 12312312312ul,
            Name                    = "Guild",
            TimezoneId              = "Europe/Warsaw",
            IconUri                 = "",
            SubscribedChannels      = new List<ulong>(),
            SubscribedReactionRules = new List<string>(),
            SubscribedResponseRules = new List<string>(),
            SubscribedCrontabRules  = new List<string>()
        };

        return createDto;
    }

    protected override void VerifySingle(GuildDocument? document, GuildDto dto)
    {
        document.Should().NotBeNull();
        document!.Id.Should().Be(dto.Id);
        VerifySingleInner(document, dto);
    }

    private static void VerifySingleInner(GuildDocument document, GuildDtoBase dto)
    {
        document.Name.Should().Be(dto.Name);
        document.DiscordId.Should().Be(dto.DiscordId);
        document.TimezoneId.Should().Be(dto.TimezoneId);
        document.IconUri.Should().Be(dto.IconUri);
        document.SubscribedChannels.Should().BeEquivalentTo(dto.SubscribedChannels);
        document.SubscribedReactionRules.Should().BeEquivalentTo(dto.SubscribedReactionRules);
        document.SubscribedResponseRules.Should().BeEquivalentTo(dto.SubscribedResponseRules);
    }

    protected override void VerifySingle(GuildDocument document, CreateGuildDto dto)
    {
        document.Should().NotBeNull();
        VerifySingleInner(document, dto);
    }

    protected override async Task<GuildDocument?> GetFromDb(string id)
    {
        return await GetServiceFromPierogiesBotHost<IMongoRepository<GuildDocument>>()
                  .GetByIdAsync(id);
    }

    protected override UpdateGuildDto GetUpdateDto()
    {
        var updateDto = new UpdateGuildDto
        {
            DiscordId               = 12312312312ul,
            Name                    = "Guild",
            TimezoneId              = "Europe/Warsaw",
            IconUri                 = "",
            SubscribedChannels      = new List<ulong>(),
            SubscribedReactionRules = new List<string>(),
            SubscribedResponseRules = new List<string>(),
            SubscribedCrontabRules  = new List<string>()
        };

        return updateDto;
    }

    protected override void VerifySingle(GuildDocument? document, UpdateGuildDto dto)
    {
        document.Should().NotBeNull();
        VerifySingleInner(document!, dto);
    }
}