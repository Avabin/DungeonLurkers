using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shared.Features;
using Shared.Features.Authentication;
using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents;
using Shared.Persistence.Identity.Features.Users;

namespace Tests.Shared;

public interface IAuthenticatedControllerTests
{
    UserDocument TestUser { get; }
    string       Password { get; }

    Task            ClearCollection<T>(IMongoClient    client) where T : IDocument<string>;
    Task            ClearUsersButTestUser(IMongoClient usersMongoClient);
    Task            CreateDocument<T>(T                document,  IMongoClient client) where T : IDocument<string>;
    Task            CreateDocuments<T>(IEnumerable<T>  documents, IMongoClient client) where T : IDocument<string>;
    Task<IUsersApi> GetIdentityRestClient();
    Task<(TClient, IServiceProvider)> ConfigureResourceServer<TStartup, TClient>(Action<HttpClient> backchannelSetter)
        where TStartup : class where TClient : IAuthenticatedApi;
}