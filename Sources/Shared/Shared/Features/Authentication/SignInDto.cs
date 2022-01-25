namespace Shared.Features.Authentication;

public class SignInDto : Dictionary<string, object>
{

    public string ClientId { get => (string)this["client_id"]; set => this["client_id"] = value; }

    public string ClientSecret { get => (string)this["client_secret"]; set => this["client_secret"] = value; }

    public string UserName { get => (string)this["username"]; set => this["username"] = value; }

    public string Password { get => (string)this["password"]; set => this["password"] = value; }

    public string GrantType { get => (string)this["grant_type"]; set => this["grant_type"] = value; }

    public string Scope { get => (string)this["scope"]; set => this["scope"] = value; }
}