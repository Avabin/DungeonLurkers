using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents.Many;

namespace Shared.Persistence.Identity.Features.Users.Many;

public interface IManyUsersService : IManyDocumentsService<UserDocument, string, UserDto>
{
}