using Microsoft.Extensions.Configuration;
using Shared.Features.Authentication;

namespace TheDungeonGuide.UI.Shared.Features.Login;

public class LoginService : ILoginService
{
    private readonly IAuthenticatedApi _api;
    private readonly string            _clientSecret;
    public LoginService(IAuthenticatedApi api, IConfiguration configuration)
    {
        _api          = api;
        _clientSecret = configuration["JWT:Secret"] ?? "secret";
    }
    public async Task<SignInResponse> LoginAsync(string userName, string password) => 
        await _api.AuthorizeWithPasswordAsync(userName, password, "users.*", clientSecret: _clientSecret);
}