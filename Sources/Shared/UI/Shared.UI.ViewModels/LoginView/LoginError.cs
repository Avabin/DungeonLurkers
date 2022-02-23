using Newtonsoft.Json;

namespace Shared.UI.ViewModels.LoginView;

public record LoginError(string Error, [JsonProperty("error_description")]string Description);