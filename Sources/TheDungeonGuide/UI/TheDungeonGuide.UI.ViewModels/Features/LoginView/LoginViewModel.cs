using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shared.Features.Authentication;
using TheDungeonGuide.UI.Shared.Features.HostScreen;
using TheDungeonGuide.UI.Shared.Features.Navigation.RoutableViewModel;
using TheDungeonGuide.UI.ViewModels.Features.ProfileView;

namespace TheDungeonGuide.UI.ViewModels.Features.LoginView;

public class LoginViewModel : RoutableViewModelBase
{
    private readonly Lazy<IAuthenticatedApi> _api;
    private          IAuthenticatedApi       Api => _api.Value;

    [Reactive] public string Username { get; set; }


    public ReactiveCommand<string, Unit>             LoginCommand { get; set; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoToProfile  { get; }

    public LoginViewModel(Lazy<IAuthenticatedApi> api, IHostScreenViewModel hostScreenViewModel) :
        base(hostScreenViewModel)
    {
        _api = api;

        LoginCommand = ReactiveCommand.CreateFromTask<string>(LoginAsync);

        GoToProfile = CreateNavigateCommand<ProfileViewModel>();
    }

    private async Task LoginAsync(string password)
    {
        await Api.SignInAsync(new PasswordSignInDto() { UserName = Username, Password = password });

        GoToProfile.Execute().Subscribe();
    }

    public override string UrlPathSegment => "login";
}