namespace Shared.UI.Authentication;

public class AuthenticationState
{
    public string Id { get; } = Guid.Parse("5DEDB515-05CF-426B-8342-C3AB1C4DA2C8").ToString();
    public string?        Token      { get; set; }
    public DateTimeOffset Expiration { get; set; }
}