using AspNetCore.Identity.Mongo.Model;
using Shared.Features;
using Shared.Persistence.Core.Features.Documents;

namespace Shared.Persistence.Identity.Features.Roles;

public class RoleDocument : MongoRole<string>, IDocument<string>
{
}