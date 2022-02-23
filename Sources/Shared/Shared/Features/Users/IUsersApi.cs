using RestEase;
using Shared.Features.Authentication;

namespace Shared.Features.Users;

public interface IUsersApi : IAuthenticatedApi
{
    [Get("users/{id}")] Task<UserDto> GetUserByIdAsync([Path] string id);

    [Get("users/UserName/{username}")] Task<UserDto> GetUserByUsernameAsync([Path] string username);

    [Post("users")] Task<UserDto> CreateUserAsync([Body] CreateUserDto user);

    [Put("users/{id}")] Task UpdateUserAsync([Path] string id, [Body] UpdateUserDto user);

    [Delete("users/{id}")] Task DeleteUserAsync([Path] string id);
}