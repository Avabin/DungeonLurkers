using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents;

namespace Shared.Persistence.Identity.Features.Users;

public sealed class UserDocument : MongoUser<string>, IDocument<string>
{
    public UserDocument() => Id = ObjectId.GenerateNewId().ToString();
}