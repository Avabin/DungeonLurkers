using System.Net.Http.Headers;
using RestEase;
using Shared.Features.Users;

namespace Shared.Features.Authentication;

public interface IAuthenticatedApi
{
    [Header("Authorization")] AuthenticationHeaderValue Authorization { get; set; }

    [Post("connect/token")]
    Task<SignInResponse> SignInAsync([Body(BodySerializationMethod.UrlEncoded)] SignInDto signIn);

    [Get("users/me")] Task<UserDto> GetCurrentUserProfileAsync();
}