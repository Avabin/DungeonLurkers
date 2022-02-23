using System.Net.Http.Headers;
using RestEase;
using Shared.Features.Users;

namespace Shared.Features.Authentication;

public interface IAuthenticatedApi : IHasPathPrefix

{
    [Path("IdentityPathPrefix")] 
    string                    IdentityPathPrefix { get; set; }
    [Header("Authorization")]           AuthenticationHeaderValue Authorization      { get; set; }

    [Post("{IdentityPathPrefix}/connect/token")]
    Task<SignInResponse> SignInAsync([Body(BodySerializationMethod.UrlEncoded)] SignInDto signIn);

    [Get("{IdentityPathPrefix}/users/me")] Task<UserDto> GetCurrentUserProfileAsync();
}