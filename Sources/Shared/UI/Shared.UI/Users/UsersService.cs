using Shared.Features.Users;

namespace Shared.UI.Users;

internal class UsersService : IUsersService
{
    private readonly IUsersApi _api;

    public UsersService(IUsersApi api)
    {
        _api = api;
    }
    public async Task<UserDto> GetUserByName(string name)
    {
        return await _api.GetUserByUsernameAsync(name);
    }

    public async Task<UserDto> GetCurrentUser()
    {
        return await _api.GetCurrentUserProfileAsync();
    }
}