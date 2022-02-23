using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Shared.Features.Authentication;

namespace Shared.UI.Authentication;

/// <summary>
///     This class binds the authentication state to the message bus.
///     When the token expires, the message bus will be notified.
///     To handle the message, the application should listen
///     for <see cref="UserLoginNeeded" /> and
///     display login view for user to login again.
/// </summary>
public class AuthenticationHostedService : IHostedService
{
    private readonly IAuthenticationStore                 _authenticationStore;
    private readonly IAuthenticatedApi                    _api;
    private readonly ILogger<AuthenticationHostedService> _logger;
    private readonly IMessageBus                          _messageBus;
    private          CompositeDisposable?                 _sub;

    public AuthenticationHostedService(ILogger<AuthenticationHostedService> logger, IMessageBus messageBus,
        IAuthenticationStore authenticationStore, IAuthenticatedApi api)
    {
        _logger              = logger;
        _messageBus          = messageBus;
        _authenticationStore = authenticationStore;
        _api            = api;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _sub = new CompositeDisposable();
        var tokenExpiredObservable = _authenticationStore.AuthenticationObservable
            .ObserveOn(RxApp.TaskpoolScheduler)
            .WhereNotNull()
            .Select(x => Observable.Return(x).Delay(x.Expiration))
            .Switch()
            .Select(x => false);
        var loginNeededObservable = _authenticationStore.IsAuthenticated
            .Skip(1) // skip initial value which is emitted always as false due to BehaviourSubject
            .Merge(tokenExpiredObservable)
            .Where(x => !x) // Not authenticated
            .Do(_ =>
            {
                _logger.LogInformation("Login expired or not found, requesting new user login");
                _messageBus.SendMessage(new UserLoginNeeded());
            });
        loginNeededObservable.Subscribe().DisposeWith(_sub);
        _authenticationStore
           .AuthenticationObservable
           .WhereNotNull()
           .Do(x => _api.SetBearer(x.Token!))
           .Subscribe()
           .DisposeWith(_sub);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _sub?.Dispose();
        return Task.CompletedTask;
    }
}