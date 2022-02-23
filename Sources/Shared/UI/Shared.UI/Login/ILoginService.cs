using Shared.Features.Authentication;

namespace Shared.UI.Login;

public interface ILoginService
{
    Task<SignInResponse> LoginAsync(string userName, string password);
}