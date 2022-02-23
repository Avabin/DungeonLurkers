using RestEase;
using Shared.Features.Authentication;

namespace Shared.Features.Users;

public interface IUsersApi : IAuthenticatedApi
{
    [Get("{IdentityPathPrefix}/users/{id}")] Task<UserDto> GetUserByIdAsync([Path] string id);

    [Get("{IdentityPathPrefix}/users/UserName/{username}")] Task<UserDto> GetUserByUsernameAsync([Path] string username);

    [Post("{IdentityPathPrefix}/users")] Task<UserDto> CreateUserAsync([Body] CreateUserDto user);

    [Put("{IdentityPathPrefix}/users/{id}")] Task UpdateUserAsync([Path] string id, [Body] UpdateUserDto user);

    [Delete("{IdentityPathPrefix}/users/{id}")] Task DeleteUserAsync([Path] string id);
}