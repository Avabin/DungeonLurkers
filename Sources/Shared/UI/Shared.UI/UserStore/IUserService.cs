using Shared.Features.Users;

namespace Shared.UI.UserStore;

public interface IUserService
{
    IObservable<UserDto> UserInfoObservable { get; }

    IObservable<UserDto> FetchProfile();
}