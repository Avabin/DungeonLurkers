using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents.Single;

namespace Shared.Persistence.Identity.Features.Users.Single;

public interface ISingleUserService : ISingleDocumentService<UserDocument, string, UserDto>
{
    Task<UserDto?> GetByUsernameAsync(string username);
}