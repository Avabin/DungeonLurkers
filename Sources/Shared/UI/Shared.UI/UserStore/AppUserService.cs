using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using RestEase;
using Shared.Features.Authentication;
using Shared.Features.Users;
using Shared.UI.Authentication;

namespace Shared.UI.UserStore;

public class AppUserService : IUserService
{
    private readonly IAuthenticatedApi    _api;
    private readonly ISubject<UserDto?>   _userInfoSubject;
    /// Emits only when the user is logged in (profile response is not null).
    public           IObservable<UserDto> UserInfoObservable => _userInfoSubject.WhereNotNull().AsObservable();

    public AppUserService(IAuthenticatedApi api)
    {
        _api             = api;
        _userInfoSubject = new BehaviorSubject<UserDto?>(null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Single value Observable</returns>
    public IObservable<UserDto> FetchProfile() =>
        _api.GetCurrentUserProfileAsync()
            .ToObservable()
            .Do(user => _userInfoSubject.OnNext(user));
}