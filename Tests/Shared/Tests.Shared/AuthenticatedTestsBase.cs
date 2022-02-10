using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Identity.Host;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Mongo2Go;
using MongoDB.Driver;
using RestEase;
using Shared.Features;
using Shared.Features.Authentication;
using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Documents;

namespace Tests.Shared;

[SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
public abstract class AuthenticatedTestsBase : IAuthenticatedControllerTests, IDisposable
{
    protected        Guid                            TestUserSuffix = Guid.NewGuid();
    private readonly List<IDisposable>               _disposables   = new();
    private readonly List<MongoDbRunner>             _mongos        = new();
    private          WebApplicationFactory<Startup>? _identityWaf;

    public abstract string       ClientId     { get; }
    public abstract string       ClientSecret { get; }
    public virtual  string       Scope        { get; } = "users.*";
    public abstract UserDocument TestUser     { get; }
    public abstract string       Password     { get; }

    public async Task ClearCollection<T>(IMongoClient client) where T : IDocument<string>
    {
        var collectionName = MongoDbHelper.GetCollectionName<T>();
        await client
             .GetDatabase(MongoDbHelper.DatabaseName)
             .GetCollection<T>(collectionName)
             .DeleteManyAsync(FilterDefinition<T>.Empty);
    }

    public async Task ClearUsersButTestUser(IMongoClient usersMongoClient)
    {
        var usersCollectionName = MongoDbHelper.GetCollectionName<UserDocument>();
        await usersMongoClient
             .GetDatabase(MongoDbHelper.DatabaseName)
             .GetCollection<UserDocument>(usersCollectionName)
             .DeleteManyAsync(x => x.UserName != TestUser.UserName);
    }

    public async Task CreateDocument<T>(T document, IMongoClient client) where T : IDocument<string>
    {
        var collectionName = MongoDbHelper.GetCollectionName<T>();
        var collection     = client.GetDatabase(MongoDbHelper.DatabaseName).GetCollection<T>(collectionName);
        await collection.InsertOneAsync(document);
    }

    public async Task CreateDocuments<T>(IEnumerable<T> documents, IMongoClient client) where T : IDocument<string>
    {
        var collectionName = MongoDbHelper.GetCollectionName<T>();
        var collection     = client.GetDatabase(MongoDbHelper.DatabaseName).GetCollection<T>(collectionName);
        await collection.InsertManyAsync(documents);
    }

    public async Task<IUsersApi> GetIdentityRestClient()
    {
        if (_identityWaf is null)
        {
            var mongo = MongoDbRunner.Start();
            _mongos.Add(mongo);
            _identityWaf =
                new WebApplicationFactory<Startup>()
                   .WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                                           configurationBuilder
                                              .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                                               {
                                                   new("ConnectionStrings:MongoDb",
                                                       mongo
                                                          .ConnectionString)
                                               })));
        }

        var usersClient = _identityWaf.CreateClient();
        await EnsureTestUserCreatedAsync();
        var usersRestClient = RestClient.For<IUsersApi>(usersClient);
        await usersRestClient.AuthorizeWithPasswordAsync(TestUser.UserName, Password, Scope, ClientId, ClientSecret);
        return usersRestClient;
    }

    public async Task<(TClient, IServiceProvider)> ConfigureResourceServer<TStartup, TClient>(
        Action<HttpClient> backchannelSetter) where TStartup : class where TClient : IAuthenticatedApi
    {
        var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
        var executingDirectory        = Path.GetFullPath("..", executingAssemblyLocation);
        var fileProvider              = new PhysicalFileProvider(executingDirectory);

        // Create identity server if not exists
        if (_identityWaf is null)
        {
            var mongo = MongoDbRunner.Start();
            _mongos.Add(mongo);
            _identityWaf =
                new WebApplicationFactory<Startup>()
                   .WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                                           configurationBuilder
                                              .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                                               {
                                                   new("ConnectionStrings:MongoDb",
                                                       mongo
                                                          .ConnectionString)
                                               })
                                              .SetFileProvider(fileProvider)));
        }

        var identityClient = _identityWaf.CreateClient();
        await EnsureTestUserCreatedAsync();
        // Invoke setter to set resource server HttpClient with configured identity client
        backchannelSetter?.Invoke(identityClient);
        // Create resource server
        var resourceMongo = MongoDbRunner.Start();
        _mongos.Add(resourceMongo);
        var resourceWaf = new WebApplicationFactory<TStartup>()
           .WithWebHostBuilder(builder => builder
                                  .ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder
                                                                .AddInMemoryCollection(new List<KeyValuePair<string,
                                                                     string>>
                                                                 {
                                                                     new("ConnectionStrings:MongoDb",
                                                                         resourceMongo
                                                                            .ConnectionString)
                                                                 })
                                                                .SetFileProvider(fileProvider)));
        // Store all created resource  servers for later dispose
        _disposables.Add(resourceWaf);
        // Create strongly typed clients
        var usersRestClient = RestClient.For<IUsersApi>(identityClient);
        var resourceClient  = RestClient.For<TClient>(resourceWaf.CreateClient());

        // Try to authorize as TestUser
        var result =
            await usersRestClient.AuthorizeWithPasswordAsync(TestUser.UserName, Password, Scope, ClientId,
                                                             ClientSecret);
        // Set received bearer token to resource client to be able to call protected resources
        resourceClient.SetBearer(result.AccessToken);

        return (resourceClient, resourceWaf.Services);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected T GetIdentityHostService<T>() where T : notnull => _identityWaf!.Services.GetRequiredService<T>();

    private async Task EnsureTestUserCreatedAsync()
    {
        var userManager = GetIdentityHostService<UserManager<UserDocument>>();

        var savedUser = await userManager.FindByNameAsync(TestUser.UserName);

        // Create user if it doesn't exist
        if (savedUser == null) await userManager.CreateAsync(TestUser, Password);

        // Change user password if it doesn't match
        if (!await userManager.CheckPasswordAsync(TestUser, Password))
        {
            await userManager.RemovePasswordAsync(TestUser);
            await userManager.AddPasswordAsync(TestUser, Password);
        }
    }

    public virtual async Task TearDown()
    {
        if (_identityWaf is not null)
        {
            await _identityWaf.DisposeAsync();
            _identityWaf = null;
        }

        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        foreach (var mongoDbRunner in _mongos)
        {
            mongoDbRunner.Dispose();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _identityWaf?.Dispose();
        foreach (var disposable in _disposables) disposable.Dispose();
    }
}