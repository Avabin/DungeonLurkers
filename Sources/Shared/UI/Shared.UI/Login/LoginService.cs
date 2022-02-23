using Microsoft.Extensions.Configuration;
using Shared.Features.Authentication;
using Shared.UI.Authentication;
using Shared.UI.UserStore;

namespace Shared.UI.Login;

public class LoginService : ILoginService
{
    private readonly IAuthenticationStore _authenticationStore;
    private readonly IAuthenticatedApi    _api;
    private readonly string               _clientSecret;

    public LoginService(IAuthenticationStore authenticationStore, IAuthenticatedApi api,
                        IConfiguration configuration)
    {
        _authenticationStore = authenticationStore;
        _api                 = api;
        _clientSecret        = configuration["JWT:Secret"] ?? "secret";
    }

    public async Task<SignInResponse> LoginAsync(string userName, string password, string scope = "IdentityServerApi")
    {
        var result =
            await _api.AuthorizeWithPasswordAsync(userName, password, scope, clientSecret: _clientSecret);

        await _authenticationStore.PublishToken(result.AccessToken,
                                                DateTimeOffset.Now.AddSeconds(result.ExpiresInSeconds));

        return result;
    }
}