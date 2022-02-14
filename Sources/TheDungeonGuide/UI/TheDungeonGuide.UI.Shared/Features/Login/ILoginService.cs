using Shared.Features.Authentication;

namespace TheDungeonGuide.UI.Shared.Features.Login;

public interface ILoginService
{
    Task<SignInResponse> LoginAsync(string userName, string password);
}