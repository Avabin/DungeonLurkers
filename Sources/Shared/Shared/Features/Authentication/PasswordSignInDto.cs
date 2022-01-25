namespace Shared.Features.Authentication;

public class PasswordSignInDto : SignInDto
{
    public PasswordSignInDto() => GrantType = "password";
}