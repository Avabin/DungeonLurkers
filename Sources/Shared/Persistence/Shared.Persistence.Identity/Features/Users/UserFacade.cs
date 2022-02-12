using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Identity.Features.Users.Many;
using Shared.Persistence.Identity.Features.Users.Single;

namespace Shared.Persistence.Identity.Features.Users;

public class UserFacade : DocumentFacade<UserDocument, string, UserDto>, IUserFacade
{
    private readonly ISingleUserService _singleDocumentService;

    public UserFacade(
        ISingleUserService singleDocumentService,
        IManyUsersService  manyDocumentsService) :
        base(singleDocumentService, manyDocumentsService) =>
        _singleDocumentService = singleDocumentService;

    public Task<UserDto?> GetByUsernameAsync(string username)
    {
        return _singleDocumentService.GetByUsernameAsync(username);
    }
}