using Blazored.LocalStorage;
using Shared.Features.Authentication;
using Shared.UI.Authentication;
using Shared.UI.Users;
using Shared.UI.UserStore;

namespace Shared.UI.Web;

public class LocalStorageAuthenticationStore : AuthenticationStoreBase
{
    private readonly ILocalStorageService _localStorageService;

    public LocalStorageAuthenticationStore(
        ILocalStorageService localStorageService, IAuthenticatedApi api) : base(api)
    {
        _localStorageService = localStorageService;
    }

    protected override async Task SaveToken(AuthenticationState state)
    {
        await _localStorageService.SetItemAsync("authentication", state);
    }

    protected override async Task<AuthenticationState?> LoadToken()
    {
        var item = await _localStorageService.GetItemAsync<AuthenticationState>("authentication");
        return item;
    }
}