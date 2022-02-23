using Microsoft.Extensions.Configuration;
using Shared.Features.Authentication;
using Shared.UI.UserStore;

namespace Shared.UI.Login;

public class LoginService : ILoginService
{
    private readonly IUserStore        _userStore;
    private readonly IAuthenticatedApi _api;
    private readonly string            _clientSecret;
    public LoginService(IUserStore userStore, IAuthenticatedApi api, IConfiguration configuration)
    {
        _userStore = userStore;
        _api            = api;
        _clientSecret   = configuration["JWT:Secret"] ?? "secret";
    }
    public async Task<SignInResponse> LoginAsync(string userName, string password)
    {
        var result = await _api.AuthorizeWithPasswordAsync(userName, password, "IdentityServerApi sessions.* characters.*", clientSecret: _clientSecret);
        
        _userStore.PublishUserInfo(await _api.GetCurrentUserProfileAsync());

        return result;
    }
}