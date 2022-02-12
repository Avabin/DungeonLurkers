using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents.Many;

namespace Shared.Persistence.Identity.Features.Users;

public interface IUserFacade : IDocumentFacade<UserDocument, string, UserDto>
{
    Task<UserDto?> GetByUsernameAsync(string username);
}