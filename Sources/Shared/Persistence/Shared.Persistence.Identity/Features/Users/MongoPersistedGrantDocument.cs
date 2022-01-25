using IdentityServer4.Models;
using MongoDB.Bson;
using Shared.Persistence.Core.Features.Documents;

namespace Shared.Persistence.Identity.Features.Users;

public class MongoPersistedGrantDocument : PersistedGrant, IDocument<string>
{
    public MongoPersistedGrantDocument(PersistedGrant grant)
    {
        Data         = grant.Data;
        Description  = grant.Description;
        Expiration   = grant.Expiration;
        Key          = grant.Key;
        Type         = grant.Type;
        ClientId     = grant.ClientId;
        ConsumedTime = grant.ConsumedTime;
        CreationTime = grant.CreationTime;
        SessionId    = grant.SessionId;
        SubjectId    = grant.SubjectId;
    }
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
}