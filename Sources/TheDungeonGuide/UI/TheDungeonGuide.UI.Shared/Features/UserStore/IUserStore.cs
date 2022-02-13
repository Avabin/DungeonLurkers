using Shared.Features.Users;

namespace TheDungeonGuide.UI.Shared.Features.UserStore;

public interface IUserStore
{
    IObservable<UserDto> UserInfoObservable { get; }
}