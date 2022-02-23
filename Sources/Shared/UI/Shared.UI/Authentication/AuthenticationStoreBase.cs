using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using RestEase;
using Shared.Features.Authentication;
using Shared.UI.Users;
using Shared.UI.UserStore;

namespace Shared.UI.Authentication;

public abstract class AuthenticationStoreBase : IAuthenticationStore
{
    private readonly IUsersService     _usersService;
    private readonly IAuthenticatedApi _api;

    private readonly ISubject<AuthenticationState?>    _subject;
    public           IObservable<AuthenticationState?> AuthenticationObservable => _subject.AsObservable();
    public           IObservable<bool>    IsAuthenticated => AuthenticationObservable.Select(x => x?.Expiration >= DateTimeOffset.Now && !string.IsNullOrWhiteSpace(x.Token));

    protected AuthenticationStoreBase(IAuthenticatedApi api)
    {
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
        }
        _subject.OnNext(saved);
    }

    protected abstract Task                 SaveToken(AuthenticationState state);
    protected abstract Task<AuthenticationState?> LoadToken();
}