namespace Shared.UI.Authentication;

public interface IAuthenticationStore
{
    IObservable<AuthenticationState?> TokenObservable { get; }
    IObservable<bool>    IsAuthenticated { get; }

    Task PublishToken(string token, DateTimeOffset expiration);
    Task Initialize();
}