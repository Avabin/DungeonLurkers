using Shared.Features.Users;

namespace Shared.UI.Users;

public interface IUsersService
{
    Task<UserDto> GetUserByName(string name);

    Task<UserDto> GetCurrentUser();
    
}