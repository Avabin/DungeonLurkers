using Shared.Features.Authentication;
using Shared.Features.Users;

namespace TheDungeonGuide.UI.Shared.Features.UserStore;

public class AppUserStore : IUserStore
{
    private readonly Lazy<IAuthenticatedApi> _api;
    private          IAuthenticatedApi       Api                => _api.Value;
    public           IObservable<UserDto>    UserInfoObservable { get; }

    public AppUserStore(Lazy<IAuthenticatedApi> api)
    {
        _api = api;

        UserInfoObservable = System.Reactive.Linq.Observable.FromAsync(Api.GetCurrentUserProfileAsync);
    }
}