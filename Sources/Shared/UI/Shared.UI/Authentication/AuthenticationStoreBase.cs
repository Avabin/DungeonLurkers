using System.Reactive.Linq;
using System.Reactive.Subjects;
using Shared.Features.Authentication;
using Shared.UI.Users;
using Shared.UI.UserStore;

namespace Shared.UI.Authentication;

public abstract class AuthenticationStoreBase : IAuthenticationStore
{
    private readonly IUserStore        _userStore;
    private readonly IUsersService     _usersService;
    private readonly IAuthenticatedApi _api;

    private readonly ISubject<AuthenticationState?>    _subject;
    public           IObservable<AuthenticationState?> TokenObservable => _subject.AsObservable();
    public           IObservable<bool>    IsAuthenticated => TokenObservable.Select(x => x?.Expiration >= DateTimeOffset.Now && !string.IsNullOrWhiteSpace(x.Token));

    protected AuthenticationStoreBase(IUserStore userStore, IUsersService usersService, IAuthenticatedApi api)
    {
        _userStore    = userStore;
        _usersService = usersService;
        _api     = api;
        _subject      = new BehaviorSubject<AuthenticationState?>(null);

    }

    public async Task PublishToken(string token, DateTimeOffset expiration)
    {
        var authenticationState = new AuthenticationState
        {
            Expiration = expiration,
            Token      = token
        };
        await SaveToken(authenticationState);
        _subject.OnNext(authenticationState);
    }

    public async Task Initialize()
    {
        var saved = await LoadToken();
        if (saved != null)
        {
            _api.SetBearer(saved.Token);
            var user = await _usersService.GetCurrentUser();
            _userStore.PublishUserInfo(user);
        }
        _subject.OnNext(saved);
    }

    protected abstract Task                 SaveToken(AuthenticationState state);
    protected abstract Task<AuthenticationState?> LoadToken();
}