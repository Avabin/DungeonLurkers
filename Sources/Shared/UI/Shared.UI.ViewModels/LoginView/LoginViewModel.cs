using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.States;
using RestEase;
using Shared.UI.HostScreen;
using Shared.UI.Login;
using Shared.UI.Navigation.RoutableViewModel;
using Shared.UI.Observables;
using Shared.UI.ViewModels.ProfileView;

namespace Shared.UI.ViewModels.LoginView;

public class LoginViewModel : ViewModelBase
{
    private readonly ILoginService        _api;

    [Reactive] public string? Username { get; set; }

    [Reactive] public string? Password { get; set; }

    public ReactiveCommand<Unit, Unit>               LoginCommand { get; set; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToProfile  { get; }

    public LoginViewModel(ILoginService api, IHostScreenViewModel hostScreenViewModel) :
        base(hostScreenViewModel)
    {
        _api                      = api;

        GoToProfile = hostScreenViewModel.CreateNavigateAndResetCommand<ProfileViewModel>();

        var usernameIsValid = this.WhenAnyValue(vm => vm.Username).Select(string.IsNullOrWhiteSpace).Toggle();
        var passwordIsValid = this.WhenAnyValue(vm => vm.Password).Select(string.IsNullOrWhiteSpace).Toggle();

        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync, usernameIsValid.Zip(passwordIsValid).Select(tuple => tuple.First && tuple.Second));


        var loginExceptionMessageObservable = LoginCommand
                                             .ThrownExceptions
                                             .OfType<HttpRequestException>()
                                             .Select(x => new ValidationState(false, x.Message));
        
        var loginErrorObservable = LoginCommand
                                  .ThrownExceptions
                                  .OfType<ApiException>()
                                  .Select(ex => ex.Content)
                                  .WhereNotNull()
                                  .Select(JsonConvert.DeserializeObject<LoginError>)
                                  .Select(x => new ValidationState(false, x.Description))
                                  .Merge(loginExceptionMessageObservable);

        this.ValidationRule(vm => vm.Password, loginErrorObservable);
        this.ValidationRule(vm => vm.Username, usernameIsValid, "You must enter a username");
        this.ValidationRule(vm => vm.Password, passwordIsValid, "You must enter a password");

    }

    private async Task LoginAsync()
    {
        await _api.LoginAsync(Username!, Password!);
    }

    public override string             UrlPathSegment => "login";
}