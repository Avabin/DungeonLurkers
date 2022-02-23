using Shared.Features.Users;

namespace Shared.UI.UserStore;

public interface IUserStore
{
    IObservable<UserDto> UserInfoObservable { get; }

    void PublishUserInfo(UserDto userInfo);
}