using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents.Many;
using Shared.Persistence.Identity.Features.Users.Many;
using Shared.Persistence.Identity.Features.Users.Single;

namespace Shared.Persistence.Identity.Features.Users;

public class UserFacade : DocumentOperationFacade<UserDocument, string, UserDto>, IUserFacade
{
    private readonly ISingleUserService _singleSingleDocumentService;

    public UserFacade(
        ISingleUserService singleSingleDocumentService,
        IManyUsersService  manyManyDocumentsService) :
        base(singleSingleDocumentService, manyManyDocumentsService) =>
        _singleSingleDocumentService = singleSingleDocumentService;

    public Task<UserDto?> GetByUsernameAsync(string username)
    {
        return _singleSingleDocumentService.GetByUsernameAsync(username);
    }
}