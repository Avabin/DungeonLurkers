using System.Net.Http.Headers;

namespace Shared.Features.Authentication;

public static class AuthenticatedApiExtensions
{
    public static async Task<SignInResponse> AuthorizeWithPasswordAsync(
        this IAuthenticatedApi client,
        string                 userName,
        string                 password,
        string                 scope,
        string                 clientId     = "default",
        string                 clientSecret = "secret")
    {
        var response = await client.SignInAsync(new PasswordSignInDto
        {
            ClientId     = clientId,
            ClientSecret = clientSecret,
            Scope        = scope,
            UserName     = userName,
            Password     = password,
        });

        client.SetBearer(response.AccessToken);

        return response;
    }

    public static void SetBearer(this IAuthenticatedApi client, string accessToken)
    {
        client.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}